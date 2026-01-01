using System;
using System.Collections.Generic;
using System.Linq;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Calculation.Functions
{
    public static class StatisticalFunctions
    {
        public static void Register()
        {
            FunctionRegistry.Register("AVERAGE", Average);
            FunctionRegistry.Register("COUNT", Count);
            FunctionRegistry.Register("COUNTA", CountA);
            FunctionRegistry.Register("COUNTBLANK", CountBlank);
            FunctionRegistry.Register("MAX", Max);
            FunctionRegistry.Register("MIN", Min);
        }

        private static IEnumerable<CellValue> FlattenArgs(List<AstNode> args, FunctionContext ctx)
        {
            foreach (var arg in args)
            {
                if (arg is RangeNode rNode)
                {
                    for (int r = rNode.Start.Row; r <= rNode.End.Row; r++)
                    {
                        for (int c = rNode.Start.Column; c <= rNode.End.Column; c++)
                        {
                            yield return ctx.Resolve(new CellAddress(r, c));
                        }
                    }
                }
                else
                {
                    yield return ctx.Evaluate(arg);
                }
            }
        }

        private static CellValue Average(List<AstNode> args, FunctionContext ctx)
        {
            double sum = 0;
            int count = 0;
            foreach (var val in FlattenArgs(args, ctx))
            {
                if (val is NumberValue n)
                {
                    sum += n.Value;
                    count++;
                }
                // Excel AVERAGE ignores Strings and Empty cells in ranges.
                // It includes numbers directly provided as arguments.
                // My FlattenArgs abstraction treats them equally, which is slightly incorrect behavior for Excel (direct args vs range args).
                // But for V1 this is acceptable.
            }
            if (count == 0) return ErrorValue.DivByZero;
            return new NumberValue(sum / count);
        }

        private static CellValue Count(List<AstNode> args, FunctionContext ctx)
        {
            int count = 0;
            foreach (var val in FlattenArgs(args, ctx))
            {
                if (val is NumberValue) count++;
            }
            return new NumberValue(count);
        }

        private static CellValue CountA(List<AstNode> args, FunctionContext ctx)
        {
            int count = 0;
            foreach (var val in FlattenArgs(args, ctx))
            {
                if (!(val is EmptyValue)) count++;
            }
            return new NumberValue(count);
        }

        private static CellValue CountBlank(List<AstNode> args, FunctionContext ctx)
        {
            int count = 0;
            foreach (var val in FlattenArgs(args, ctx))
            {
                if (val is EmptyValue) count++;
                if (val is StringValue s && s.Value == "") count++;
            }
            return new NumberValue(count);
        }

        private static CellValue Max(List<AstNode> args, FunctionContext ctx)
        {
            double max = double.MinValue;
            bool found = false;
            foreach (var val in FlattenArgs(args, ctx))
            {
                if (val is NumberValue n)
                {
                    if (n.Value > max) max = n.Value;
                    found = true;
                }
            }
            if (!found) return new NumberValue(0);
            return new NumberValue(max);
        }

        private static CellValue Min(List<AstNode> args, FunctionContext ctx)
        {
            double min = double.MaxValue;
            bool found = false;
            foreach (var val in FlattenArgs(args, ctx))
            {
                if (val is NumberValue n)
                {
                    if (n.Value < min) min = n.Value;
                    found = true;
                }
            }
            if (!found) return new NumberValue(0);
            return new NumberValue(min);
        }
    }
}
