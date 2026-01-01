using System;
using System.Collections.Generic;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Calculation
{
    // Context passed to every function execution
    public class FunctionContext
    {
        private readonly Func<AstNode, CellValue> _evaluator;
        private readonly Func<CellAddress, CellValue> _referenceResolver;

        public FunctionContext(Func<AstNode, CellValue> evaluator, Func<CellAddress, CellValue> referenceResolver)
        {
            _evaluator = evaluator;
            _referenceResolver = referenceResolver;
        }

        public CellValue Evaluate(AstNode node) => _evaluator(node);
        public CellValue Resolve(CellAddress addr) => _referenceResolver(addr);
    }

    public delegate CellValue FunctionDelegate(List<AstNode> args, FunctionContext context);

    public static class FunctionRegistry
    {
        private static readonly Dictionary<string, FunctionDelegate> _functions = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object _lock = new();
        private static bool _initialized = false;

        public static void Register(string name, FunctionDelegate implementation)
        {
            lock (_lock)
            {
                _functions[name] = implementation;
            }
        }

        public static void Initialize()
        {
            if (_initialized) return;
            lock (_lock)
            {
                if (_initialized) return;

                Functions.MathFunctions.Register();
                Functions.LogicalFunctions.Register();
                Functions.TextFunctions.Register();
                Functions.StatisticalFunctions.Register();
                Functions.DateFunctions.Register();
                Functions.LookupFunctions.Register();
                Functions.FinancialFunctions.Register();
                Functions.InformationFunctions.Register();

                _initialized = true;
            }
        }

        public static CellValue Call(string name, List<AstNode> args, FunctionContext context)
        {
            if (_functions.TryGetValue(name, out var implementation))
            {
                try
                {
                    return implementation(args, context);
                }
                catch (Exception ex)
                {
                    return new ErrorValue($"Error in {name}: {ex.Message}");
                }
            }
            return ErrorValue.Name; // #NAME?
        }

        public static bool IsRegistered(string name) => _functions.ContainsKey(name);

        // Helper specifically for numeric arguments
        public static double GetNumericArg(AstNode node, FunctionContext ctx)
        {
            var val = ctx.Evaluate(node);
            if (val is NumberValue n) return n.Value;
            if (val is EmptyValue) return 0;
            // Attempt parse string?
            if (val is StringValue s && double.TryParse(s.Value, out double d)) return d;

            throw new ArgumentException("Expected number");
        }
    }
}
