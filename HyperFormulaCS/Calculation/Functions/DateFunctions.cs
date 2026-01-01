using System;
using System.Collections.Generic;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Calculation.Functions
{
    public static class DateFunctions
    {
        public static void Register()
        {
            FunctionRegistry.Register("DATE", Date);
            FunctionRegistry.Register("YEAR", Year);
            FunctionRegistry.Register("MONTH", Month);
            FunctionRegistry.Register("DAY", Day);
            FunctionRegistry.Register("TODAY", Today);
            FunctionRegistry.Register("NOW", Now);
            FunctionRegistry.Register("TIME", Time);
            FunctionRegistry.Register("HOUR", Hour);
            FunctionRegistry.Register("MINUTE", Minute);
            FunctionRegistry.Register("SECOND", Second);
        }

        // Excel base date: Dec 30, 1899. 
        // C# DateTime to Excel Serial
        private static double ToSerial(DateTime dt)
        {
            return (dt - new DateTime(1899, 12, 30)).TotalDays;
        }

        private static DateTime FromSerial(double serial)
        {
            return new DateTime(1899, 12, 30).AddDays(serial);
        }

        private static CellValue Date(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 3) return ErrorValue.Value;
            try
            {
                int y = (int)FunctionRegistry.GetNumericArg(args[0], ctx);
                int m = (int)FunctionRegistry.GetNumericArg(args[1], ctx);
                int d = (int)FunctionRegistry.GetNumericArg(args[2], ctx);

                var dt = new DateTime(y, m, d);
                return new NumberValue(ToSerial(dt));
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Year(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            try
            {
                double serial = FunctionRegistry.GetNumericArg(args[0], ctx);
                return new NumberValue(FromSerial(serial).Year);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Month(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            try
            {
                double serial = FunctionRegistry.GetNumericArg(args[0], ctx);
                return new NumberValue(FromSerial(serial).Month);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Day(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            try
            {
                double serial = FunctionRegistry.GetNumericArg(args[0], ctx);
                return new NumberValue(FromSerial(serial).Day);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Today(List<AstNode> args, FunctionContext ctx)
        {
            return new NumberValue(ToSerial(DateTime.Today));
        }

        private static CellValue Now(List<AstNode> args, FunctionContext ctx)
        {
            return new NumberValue(ToSerial(DateTime.Now));
        }

        private static CellValue Time(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 3) return ErrorValue.Value;
            try
            {
                int h = (int)FunctionRegistry.GetNumericArg(args[0], ctx);
                int m = (int)FunctionRegistry.GetNumericArg(args[1], ctx);
                int s = (int)FunctionRegistry.GetNumericArg(args[2], ctx);

                // Keep within valid ranges or let TimeSpan handle normalization? 
                // Excel TIME(25,0,0) -> 0.04... (drops days)
                var ts = new TimeSpan(h, m, s);
                double totalDays = ts.TotalDays;
                // Excel TIME returns value 0..0.999... (only time part)
                double decimalPart = totalDays - Math.Truncate(totalDays);
                if (decimalPart < 0) decimalPart += 1;

                return new NumberValue(decimalPart);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Hour(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            try
            {
                double serial = FunctionRegistry.GetNumericArg(args[0], ctx);
                // Serial includes date, we need hour part
                // Extract time part
                double timePart = serial - Math.Truncate(serial);
                // Handle negative or just convert
                return new NumberValue(FromSerial(serial).Hour);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Minute(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            try
            {
                double serial = FunctionRegistry.GetNumericArg(args[0], ctx);
                return new NumberValue(FromSerial(serial).Minute);
            }
            catch { return ErrorValue.Value; }
        }

        private static CellValue Second(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count != 1) return ErrorValue.Value;
            try
            {
                double serial = FunctionRegistry.GetNumericArg(args[0], ctx);
                return new NumberValue(FromSerial(serial).Second);
            }
            catch { return ErrorValue.Value; }
        }
    }
}
