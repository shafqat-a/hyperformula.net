using System.Collections.Generic;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Calculation.Functions
{
    public static class InformationFunctions
    {
        public static void Register()
        {
            FunctionRegistry.Register("ISNUMBER", IsNumber);
            FunctionRegistry.Register("ISTEXT", IsText);
            FunctionRegistry.Register("ISLOGICAL", IsLogical);
            FunctionRegistry.Register("ISERROR", IsError);
            FunctionRegistry.Register("ISBLANK", IsBlank);
        }

        private static CellValue IsNumber(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            var val = ctx.Evaluate(args[0]);
            return new BooleanValue(val is NumberValue);
        }

        private static CellValue IsText(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            var val = ctx.Evaluate(args[0]);
            return new BooleanValue(val is StringValue);
        }

        private static CellValue IsLogical(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            var val = ctx.Evaluate(args[0]);
            return new BooleanValue(val is BooleanValue);
        }

        private static CellValue IsError(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            var val = ctx.Evaluate(args[0]);
            return new BooleanValue(val is ErrorValue);
        }

        private static CellValue IsBlank(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            var val = ctx.Evaluate(args[0]);
            return new BooleanValue(val is EmptyValue || (val is StringValue s && string.IsNullOrEmpty(s.Value)));
        }
    }
}
