using System;
using System.Collections.Generic;
using System.Linq;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Calculation
{
    public class Evaluator
    {
        private readonly Func<CellAddress, CellValue> _referenceResolver;

        public Evaluator(Func<CellAddress, CellValue> referenceResolver)
        {
            _referenceResolver = referenceResolver;
        }

        public CellValue Evaluate(AstNode node)
        {
            try
            {
                return Visit(node);
            }
            catch (Exception ex)
            {
                return new ErrorValue(ex.Message);
            }
        }

        private CellValue Visit(AstNode node)
        {
            return node switch
            {
                NumberNode n => new NumberValue(n.Value),
                StringNode s => new StringValue(s.Value),
                BinaryOpNode b => EvaluateBinary(b),
                UnaryOpNode u => EvaluateUnary(u),
                CellReferenceNode r => _referenceResolver(r.Address),
                FunctionCallNode f => EvaluateFunction(f),
                _ => ErrorValue.Value
            };
        }

        private CellValue EvaluateUnary(UnaryOpNode u)
        {
            var val = Visit(u.Operand);
            if (TryGetNumber(val, out double num))
            {
                if (u.Op == "-") return new NumberValue(-num);
                if (u.Op == "+") return new NumberValue(num);
            }
            return ErrorValue.Value;
        }

        private CellValue EvaluateBinary(BinaryOpNode b)
        {
            var leftVal = Visit(b.Left);
            var rightVal = Visit(b.Right);

            // Basic type coercion: treat everything as number if possible
            if (TryGetNumber(leftVal, out double lNum) && TryGetNumber(rightVal, out double rNum))
            {
                switch (b.Op)
                {
                    case "+": return new NumberValue(lNum + rNum);
                    case "-": return new NumberValue(lNum - rNum);
                    case "*": return new NumberValue(lNum * rNum);
                    case "/":
                        if (rNum == 0) return ErrorValue.DivByZero;
                        return new NumberValue(lNum / rNum);
                    case "=": return new BooleanValue(Math.Abs(lNum - rNum) < double.Epsilon);
                    case "<>": return new BooleanValue(Math.Abs(lNum - rNum) > double.Epsilon);
                    case ">": return new BooleanValue(lNum > rNum);
                    case "<": return new BooleanValue(lNum < rNum);
                    case ">=": return new BooleanValue(lNum >= rNum);
                    case "<=": return new BooleanValue(lNum <= rNum);
                }
            }

            // string comparison etc.
            if (leftVal is StringValue ls && rightVal is StringValue rs)
            {
                int cmp = string.Compare(ls.Value, rs.Value, StringComparison.OrdinalIgnoreCase);
                switch (b.Op)
                {
                    case "=": return new BooleanValue(cmp == 0);
                    case "<>": return new BooleanValue(cmp != 0);
                        // simple string comparison support
                }
            }

            // Concatenation... (Not standard +, but could be & if intended, but let's stick to basic Excel logic)
            // Excel: + requires numbers. & is concat. 
            // If Op is not found or types don't match, return Error.

            return ErrorValue.Value;
        }

        private CellValue EvaluateFunction(FunctionCallNode f)
        {
            var context = new FunctionContext(Visit, _referenceResolver);
            return FunctionRegistry.Call(f.FunctionName, f.Arguments, context);
        }

        private bool TryGetNumber(CellValue val, out double result)
        {
            result = 0;
            switch (val)
            {
                case NumberValue n: result = n.Value; return true;
                case EmptyValue: result = 0; return true;
                // Excel behavior: Strings are 0 in SUM but Error in +
                // For simplicity: explicit string-to-number parse?
                // Let's implement strict:
                case StringValue: return false;
                default: return false;
            }
        }
    }
}
