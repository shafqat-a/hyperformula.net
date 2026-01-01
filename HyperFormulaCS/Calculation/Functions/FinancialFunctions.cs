using System;
using System.Collections.Generic;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Calculation.Functions
{
    public static class FinancialFunctions
    {
        public static void Register()
        {
            FunctionRegistry.Register("PMT", Pmt);
            FunctionRegistry.Register("FV", Fv);
            FunctionRegistry.Register("PV", Pv);
            FunctionRegistry.Register("NPV", Npv);
        }

        // Helpers
        private static double GetOptNumber(List<AstNode> args, int index, FunctionContext ctx, double defaultValue = 0)
        {
            if (index >= args.Count) return defaultValue;
            try { return FunctionRegistry.GetNumericArg(args[index], ctx); }
            catch { return defaultValue; }
        }

        private static CellValue Pmt(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count < 3 || args.Count > 5) return ErrorValue.Value;
            try
            {
                double rate = FunctionRegistry.GetNumericArg(args[0], ctx);
                double nper = FunctionRegistry.GetNumericArg(args[1], ctx);
                double pv = FunctionRegistry.GetNumericArg(args[2], ctx);
                double fv = GetOptNumber(args, 3, ctx, 0);
                double type = GetOptNumber(args, 4, ctx, 0); // 0 = end of period, 1 = begin

                if (rate == 0)
                {
                    return new NumberValue(-(pv + fv) / nper);
                }

                // PMT = (pv * (1+r)^n + fv) * r / ( (1+r*type) * ( (1+r)^n - 1 ) ) ?
                // Standard formula:
                // PMT = ( PV * r * (1+r)^n + FV * r ) / ( (1+r*type) * (1 - (1+r)^n) ) ... wait signs matter.
                // Excel: PV + PMT/r * (1 - 1/(1+r)^n) ...

                // Using standard form:
                // PV * (1+r)^n + PMT * (1+r*type) * ( (1+r)^n - 1 ) / r + FV = 0

                // PMT * term = -FV - PV*(1+r)^n
                // term = (1+r*type) * ((1+r)^n - 1) / r

                double pow = Math.Pow(1 + rate, nper);
                double term = (1 + rate * type) * (pow - 1) / rate;

                double pmt = (-fv - pv * pow) / term;
                return new NumberValue(pmt);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Fv(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count < 3 || args.Count > 5) return ErrorValue.Value;
            try
            {
                double rate = FunctionRegistry.GetNumericArg(args[0], ctx);
                double nper = FunctionRegistry.GetNumericArg(args[1], ctx);
                double pmt = FunctionRegistry.GetNumericArg(args[2], ctx);
                double pv = GetOptNumber(args, 3, ctx, 0);
                double type = GetOptNumber(args, 4, ctx, 0);

                if (rate == 0)
                {
                    return new NumberValue(-(pv + pmt * nper));
                }

                double pow = Math.Pow(1 + rate, nper);
                double fv = -(pv * pow + pmt * (1 + rate * type) * (pow - 1) / rate);
                return new NumberValue(fv);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Pv(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count < 3 || args.Count > 5) return ErrorValue.Value;
            try
            {
                double rate = FunctionRegistry.GetNumericArg(args[0], ctx);
                double nper = FunctionRegistry.GetNumericArg(args[1], ctx);
                double pmt = FunctionRegistry.GetNumericArg(args[2], ctx);
                double fv = GetOptNumber(args, 3, ctx, 0);
                double type = GetOptNumber(args, 4, ctx, 0);

                if (rate == 0)
                {
                    return new NumberValue(-(fv + pmt * nper));
                }

                double pow = Math.Pow(1 + rate, nper);
                // PV = ( -FV - PMT * term ) / pow
                double term = (1 + rate * type) * (pow - 1) / rate;
                double val = (-fv - pmt * term) / pow;
                return new NumberValue(val);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Npv(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count < 2) return ErrorValue.Value;
            try
            {
                double rate = FunctionRegistry.GetNumericArg(args[0], ctx);
                double result = 0;
                int i = 1;

                // Iterate remaining args (numbers or ranges)
                for (int idx = 1; idx < args.Count; idx++)
                {
                    var arg = args[idx];
                    if (arg is RangeNode rNode)
                    {
                        for (int r = rNode.Start.Row; r <= rNode.End.Row; r++)
                        {
                            for (int c = rNode.Start.Column; c <= rNode.End.Column; c++)
                            {
                                var val = ctx.Resolve(new CellAddress(r, c));
                                if (val is NumberValue n)
                                {
                                    result += n.Value / Math.Pow(1 + rate, i);
                                    i++;
                                }
                            }
                        }
                    }
                    else
                    {
                        double val = FunctionRegistry.GetNumericArg(arg, ctx);
                        result += val / Math.Pow(1 + rate, i);
                        i++;
                    }
                }
                return new NumberValue(result);
            }
            catch { return ErrorValue.Value; }
        }
    }
}
