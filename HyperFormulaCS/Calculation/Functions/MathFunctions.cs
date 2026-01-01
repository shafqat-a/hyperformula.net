using System;
using System.Collections.Generic;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Calculation.Functions
{
    public static class MathFunctions
    {
        private static readonly Random _random = new Random();

        public static void Register()
        {
            FunctionRegistry.Register("SUM", Sum);
            FunctionRegistry.Register("ABS", Abs);
            FunctionRegistry.Register("ACOS", Acos);
            FunctionRegistry.Register("COS", Cos);
            FunctionRegistry.Register("PI", Pi);
            FunctionRegistry.Register("POWER", Power);
            FunctionRegistry.Register("ROUND", Round);
            FunctionRegistry.Register("SQRT", Sqrt);
            FunctionRegistry.Register("INT", IntFunc);
            FunctionRegistry.Register("MOD", ModFunc);
            FunctionRegistry.Register("ROUNDUP", RoundUp);
            FunctionRegistry.Register("ROUNDDOWN", RoundDown);
            FunctionRegistry.Register("RAND", Rand);
            FunctionRegistry.Register("RANDBETWEEN", RandBetween);
        }

        private static CellValue Sum(List<AstNode> args, FunctionContext ctx)
        {
            double sum = 0;
            foreach (var arg in args)
            {
                if (arg is RangeNode rangeNode)
                {
                    // Basic range support for SUM
                    for (int r = rangeNode.Start.Row; r <= rangeNode.End.Row; r++)
                    {
                        for (int c = rangeNode.Start.Column; c <= rangeNode.End.Column; c++)
                        {
                            var cellCode = new CellAddress(r, c);
                            var val = ctx.Resolve(cellCode);
                            if (val is NumberValue n) sum += n.Value;
                        }
                    }
                }
                else
                {
                    try { sum += FunctionRegistry.GetNumericArg(arg, ctx); } catch { }
                }
            }
            return new NumberValue(sum);
        }

        private static CellValue IntFunc(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            try { return new NumberValue(Math.Floor(FunctionRegistry.GetNumericArg(args[0], ctx))); }
            catch { return ErrorValue.Value; }
        }

        private static CellValue ModFunc(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 2) return ErrorValue.Value;
            try
            {
                double n = FunctionRegistry.GetNumericArg(args[0], ctx);
                double d = FunctionRegistry.GetNumericArg(args[1], ctx);
                if (d == 0) return ErrorValue.Div0;
                return new NumberValue(n % d);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue RoundUp(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 2) return ErrorValue.Value;
            try
            {
                double number = FunctionRegistry.GetNumericArg(args[0], ctx);
                double digits = FunctionRegistry.GetNumericArg(args[1], ctx);
                double factor = Math.Pow(10, digits);
                double result = (number > 0)
                   ? Math.Ceiling(number * factor) / factor
                   : Math.Floor(number * factor) / factor; // Round away from zero? Excel ROUNDUP rounds away from 0.
                return new NumberValue(result);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue RoundDown(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 2) return ErrorValue.Value;
            try
            {
                double number = FunctionRegistry.GetNumericArg(args[0], ctx);
                double digits = FunctionRegistry.GetNumericArg(args[1], ctx);
                double factor = Math.Pow(10, digits);
                double result = (number > 0)
                   ? Math.Floor(number * factor) / factor
                   : Math.Ceiling(number * factor) / factor; // Towards zero
                return new NumberValue(result);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Rand(List<AstNode> args, FunctionContext ctx)
        {
            return new NumberValue(_random.NextDouble());
        }

        private static CellValue RandBetween(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 2) return ErrorValue.Value;
            try
            {
                int bottom = (int)FunctionRegistry.GetNumericArg(args[0], ctx);
                int top = (int)FunctionRegistry.GetNumericArg(args[1], ctx);
                if (bottom > top) return ErrorValue.Num;
                return new NumberValue(_random.Next(bottom, top + 1));
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Abs(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            try
            {
                double val = FunctionRegistry.GetNumericArg(args[0], ctx);
                return new NumberValue(Math.Abs(val));
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Acos(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            try
            {
                double val = FunctionRegistry.GetNumericArg(args[0], ctx);
                return new NumberValue(Math.Acos(val));
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Cos(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            try
            {
                double val = FunctionRegistry.GetNumericArg(args[0], ctx);
                return new NumberValue(Math.Cos(val));
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Pi(List<AstNode> args, FunctionContext ctx)
        {
            return new NumberValue(Math.PI);
        }

        private static CellValue Power(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 2) return ErrorValue.Value;
            try
            {
                double num = FunctionRegistry.GetNumericArg(args[0], ctx);
                double power = FunctionRegistry.GetNumericArg(args[1], ctx);
                return new NumberValue(Math.Pow(num, power));
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Round(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 2) return ErrorValue.Value;
            try
            {
                double num = FunctionRegistry.GetNumericArg(args[0], ctx);
                double digits = FunctionRegistry.GetNumericArg(args[1], ctx);
                return new NumberValue(Math.Round(num, (int)digits));
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Sqrt(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            try
            {
                double val = FunctionRegistry.GetNumericArg(args[0], ctx);
                if (val < 0) return ErrorValue.Num; // #NUM! for negative sqrt
                return new NumberValue(Math.Sqrt(val));
            }
            catch { return ErrorValue.Value; }
        }
    }
}
