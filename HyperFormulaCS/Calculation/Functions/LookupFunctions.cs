using System;
using System.Collections.Generic;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Calculation.Functions
{
    public static class LookupFunctions
    {
        public static void Register()
        {
            FunctionRegistry.Register("VLOOKUP", VLookup);
            FunctionRegistry.Register("MATCH", MatchFunc);
            FunctionRegistry.Register("INDEX", Index);
        }

        private static CellValue VLookup(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count < 3 || args.Count > 4) return ErrorValue.Value;

            var lookupVal = ctx.Evaluate(args[0]);
            var rangeNode = args[1] as RangeNode;
            if (rangeNode == null) return ErrorValue.Value; // Must be a range

            int colIndex;
            try { colIndex = (int)FunctionRegistry.GetNumericArg(args[2], ctx); }
            catch { return ErrorValue.Value; }

            bool approximate = true;
            if (args.Count == 4)
            {
                var rangeLookup = ctx.Evaluate(args[3]);
                if (rangeLookup is BooleanValue b) approximate = b.Value;
                else if (rangeLookup is NumberValue n) approximate = n.Value != 0;
            }

            // VLOOKUP checks first column of range
            int startRow = rangeNode.Start.Row;
            int endRow = rangeNode.End.Row;
            int startCol = rangeNode.Start.Column;

            // Validate colIndex
            if (colIndex < 1 || startCol + colIndex - 1 > rangeNode.End.Column)
                return ErrorValue.Ref;

            // Simple iteration (not optimized for binary search yet even if approximate)
            for (int r = startRow; r <= endRow; r++)
            {
                var cellVal = ctx.Resolve(new CellAddress(r, startCol));

                // Equality check
                if (IsEqual(lookupVal, cellVal)) // Strict equality for now? VLOOKUP is fuzzy loosely.
                {
                    // Found
                    return ctx.Resolve(new CellAddress(r, startCol + colIndex - 1));
                }
            }

            return ErrorValue.NA;
        }

        private static CellValue MatchFunc(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count < 2 || args.Count > 3) return ErrorValue.Value;

            var lookupVal = ctx.Evaluate(args[0]);
            var rangeNode = args[1] as RangeNode;
            if (rangeNode == null) return ErrorValue.Value;

            int matchType = 1;
            if (args.Count == 3)
            {
                try { matchType = (int)FunctionRegistry.GetNumericArg(args[2], ctx); }
                catch { return ErrorValue.Value; }
            }

            int index = 1;
            // Iterate range (assumes 1D usually, checks row by row then col by col)
            for (int r = rangeNode.Start.Row; r <= rangeNode.End.Row; r++)
            {
                for (int c = rangeNode.Start.Column; c <= rangeNode.End.Column; c++)
                {
                    var cellVal = ctx.Resolve(new CellAddress(r, c));
                    if (IsEqual(lookupVal, cellVal))
                    {
                        return new NumberValue(index);
                    }
                    index++;
                }
            }

            return ErrorValue.NA;
        }

        private static CellValue Index(List<AstNode> args, FunctionContext ctx)
        {
            if (args.Count < 2 || args.Count > 3) return ErrorValue.Value;

            var rangeNode = args[0] as RangeNode;
            if (rangeNode == null) return ErrorValue.Value;

            int rowNum;
            try { rowNum = (int)FunctionRegistry.GetNumericArg(args[1], ctx); }
            catch { return ErrorValue.Value; }

            int colNum = 1; // Default to 1 if not specified? Or 0 (entire row)? Simple Index: returns cell.
            if (args.Count == 3)
            {
                try { colNum = (int)FunctionRegistry.GetNumericArg(args[2], ctx); }
                catch { return ErrorValue.Value; }
            }

            // Relative to range
            // Row 1 is Start.Row
            int targetRow = rangeNode.Start.Row + rowNum - 1;
            int targetCol = rangeNode.Start.Column + colNum - 1;

            if (targetRow > rangeNode.End.Row || targetCol > rangeNode.End.Column)
                return ErrorValue.Ref;

            return ctx.Resolve(new CellAddress(targetRow, targetCol));
        }

        private static bool IsEqual(CellValue a, CellValue b)
        {
            // Simple value equality
            if (a is NumberValue na && b is NumberValue nb) return Math.Abs(na.Value - nb.Value) < double.Epsilon;
            if (a is StringValue sa && b is StringValue sb) return string.Equals(sa.Value, sb.Value, StringComparison.OrdinalIgnoreCase);
            if (a is BooleanValue ba && b is BooleanValue bb) return ba.Value == bb.Value;
            return false;
        }
    }
}
