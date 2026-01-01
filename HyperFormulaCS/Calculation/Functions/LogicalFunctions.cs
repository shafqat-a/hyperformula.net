using System;
using System.Collections.Generic;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Calculation.Functions
{
    public static class LogicalFunctions
    {
        public static void Register()
        {
            FunctionRegistry.Register("AND", And);
            FunctionRegistry.Register("OR", Or);
            FunctionRegistry.Register("NOT", Not);
            FunctionRegistry.Register("IF", If);
            FunctionRegistry.Register("TRUE", True);
            FunctionRegistry.Register("FALSE", False);
        }

        private static bool ToBool(CellValue val)
        {
            if (val is BooleanValue b) return b.Value;
            if (val is NumberValue n) return n.Value != 0;
            if (val is StringValue s)
            {
                if (bool.TryParse(s.Value, out bool res)) return res;
                return false; // Excel ignores text in some contexts, or errors.
            }
            return false;
        }

        private static CellValue And(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count == 0) return ErrorValue.Value;

            foreach (var arg in args)
            {
                var val = ctx.Evaluate(arg);
                if (!ToBool(val)) return new BooleanValue(false);
            }
            return new BooleanValue(true);
        }

        private static CellValue Or(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count == 0) return ErrorValue.Value;

            foreach (var arg in args)
            {
                var val = ctx.Evaluate(arg);
                if (ToBool(val)) return new BooleanValue(true);
            }
            return new BooleanValue(false);
        }

        private static CellValue Not(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            var val = ctx.Evaluate(args[0]);
            return new BooleanValue(!ToBool(val));
        }

        private static CellValue If(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count < 2 || args.Count > 3) return ErrorValue.Value;

            var cond = ctx.Evaluate(args[0]);
            if (ToBool(cond))
            {
                return ctx.Evaluate(args[1]);
            }
            else
            {
                if (args.Count == 3)
                    return ctx.Evaluate(args[2]);
                return new BooleanValue(false); // Default FALSE if 3rd arg missing
            }
        }

        private static CellValue True(List<AstNode> args, FunctionContext ctx) => new BooleanValue(true);
        private static CellValue False(List<AstNode> args, FunctionContext ctx) => new BooleanValue(false);
    }
}
