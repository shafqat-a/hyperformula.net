using System;
using System.Collections.Generic;
using System.Text;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Calculation.Functions
{
    public static class TextFunctions
    {
        public static void Register()
        {
            FunctionRegistry.Register("CONCATENATE", Concatenate);
            FunctionRegistry.Register("LEN", Len);
            FunctionRegistry.Register("LOWER", Lower);
            FunctionRegistry.Register("UPPER", Upper);
            FunctionRegistry.Register("LEFT", Left);
            FunctionRegistry.Register("RIGHT", Right);
            FunctionRegistry.Register("MID", Mid);
            FunctionRegistry.Register("TRIM", Trim);
            FunctionRegistry.Register("REPT", Rept);
            FunctionRegistry.Register("FIND", Find);
            FunctionRegistry.Register("SEARCH", Search);
            FunctionRegistry.Register("SUBSTITUTE", Substitute);
        }

        private static string GetStringArg(AstNode node, FunctionContext ctx)
        {
            var val = ctx.Evaluate(node);
            return val.ToString();
        }

        private static CellValue Concatenate(List<AstNode> args, FunctionContext ctx)
        {
            var sb = new StringBuilder();
            foreach (var arg in args)
            {
                sb.Append(GetStringArg(arg, ctx));
            }
            return new StringValue(sb.ToString());
        }

        private static CellValue Len(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            var s = GetStringArg(args[0], ctx);
            return new NumberValue(s.Length);
        }

        private static CellValue Lower(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            return new StringValue(GetStringArg(args[0], ctx).ToLowerInvariant());
        }

        private static CellValue Upper(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            return new StringValue(GetStringArg(args[0], ctx).ToUpperInvariant());
        }

        private static CellValue Left(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count < 1 || args.Count > 2) return ErrorValue.Value;
            var s = GetStringArg(args[0], ctx);
            int len = 1;
            if (args.Count == 2)
            {
                try { len = (int)FunctionRegistry.GetNumericArg(args[1], ctx); }
                catch { return ErrorValue.Value; }
            }
            if (len < 0) return ErrorValue.Value;
            if (len >= s.Length) return new StringValue(s);
            return new StringValue(s.Substring(0, len));
        }

        private static CellValue Right(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count < 1 || args.Count > 2) return ErrorValue.Value;
            var s = GetStringArg(args[0], ctx);
            int len = 1;
            if (args.Count == 2)
            {
                try { len = (int)FunctionRegistry.GetNumericArg(args[1], ctx); }
                catch { return ErrorValue.Value; }
            }
            if (len < 0) return ErrorValue.Value;
            if (len >= s.Length) return new StringValue(s);
            return new StringValue(s.Substring(s.Length - len, len));
        }

        private static CellValue Mid(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 3) return ErrorValue.Value;
            var s = GetStringArg(args[0], ctx);
            int start, len;
            try
            {
                start = (int)FunctionRegistry.GetNumericArg(args[1], ctx);
                len = (int)FunctionRegistry.GetNumericArg(args[2], ctx);
            }
            catch { return ErrorValue.Value; }

            if (start < 1 || len < 0) return ErrorValue.Value;
            start--; // 1-based index in Excel to 0-based
            if (start >= s.Length) return new StringValue("");
            if (start + len > s.Length) len = s.Length - start;

            return new StringValue(s.Substring(start, len));
        }

        private static CellValue Trim(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            var s = GetStringArg(args[0], ctx);
            return new StringValue(s.Trim());
        }

        private static CellValue Rept(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 2) return ErrorValue.Value;
            var s = GetStringArg(args[0], ctx);
            int count;
            try { count = (int)FunctionRegistry.GetNumericArg(args[1], ctx); }
            catch { return ErrorValue.Value; }

            if (count < 0) return ErrorValue.Value;
            var sb = new StringBuilder();
            for (int i = 0; i < count; i++) sb.Append(s);
            return new StringValue(sb.ToString());
        }
    }
}
