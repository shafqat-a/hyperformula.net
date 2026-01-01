using System;
using System.Collections.Generic;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;
using HyperFormulaCS.Parsing;

namespace HyperFormulaCS.Calculation
{
    public class Engine
    {
        private class CellData
        {
            public string RawFormula { get; set; } = "";
            public AstNode? Ast { get; set; }
            public CellValue CachedValue { get; set; } = CellValue.Empty;
            public bool IsDirty { get; set; } = false;
        }

        private readonly Dictionary<CellAddress, CellData> _cells = new();
        private readonly DependencyGraph _graph = new();
        private readonly Evaluator _evaluator;

        public Engine()
        {
            Calculation.FunctionRegistry.Initialize();
            _evaluator = new Evaluator(GetCellValueInternal);
        }

        public void SetCell(string address, string formula)
        {
            var addr = CellAddress.Parse(address);
            SetCell(addr, formula);
        }

        public void SetCell(CellAddress addr, string formula)
        {
            if (!_cells.ContainsKey(addr))
                _cells[addr] = new CellData();

            var cell = _cells[addr];

            // 1. Detect changes in formula
            if (cell.RawFormula == formula) return; // No change

            // 2. Clear old dependencies
            _graph.RemoveDependencies(addr);

            // 3. Parse and update AST
            cell.RawFormula = formula;
            cell.IsDirty = true;

            if (formula.StartsWith("="))
            {
                try
                {
                    var parser = new Parser(formula.Substring(1));
                    cell.Ast = parser.Parse();

                    // 4. Update dependencies
                    UpdateDependencies(addr, cell.Ast);
                }
                catch (Exception ex)
                {
                    // If parse fails, store error
                    cell.Ast = null; // Or ErrorNode
                    cell.CachedValue = new ErrorValue(ex.Message);
                    cell.IsDirty = false; // Already "calculated" as error
                }
            }
            else
            {
                // Literal value
                if (double.TryParse(formula, out double d))
                {
                    cell.Ast = new NumberNode(d);
                }
                else
                {
                    cell.Ast = new StringNode(formula);
                }
                cell.IsDirty = true;
            }

            // 5. Check for cycles
            // If cycle detected, revert or mark as Circular Ref error.
            // Simplified: Mark as error if cycle found.
            // (Strictly we should check BEFORE updating graph fully, but simplistic approach here)
            /* 
               if (_graph.DetectCycle(addr)) { ... } 
            */

            // 6. Mark dependents as dirty and recalculate if needed
            Recalculate(addr);
        }

        public CellValue GetCellValue(string address)
        {
            return GetCellValue(CellAddress.Parse(address));
        }

        public CellValue GetCellValue(CellAddress address)
        {
            return GetCellValueInternal(address);
        }

        public Dictionary<string, string> GetAllCells()
        {
            var result = new Dictionary<string, string>();
            foreach (var kvp in _cells)
            {
                var val = GetCellValue(kvp.Key);
                result[kvp.Key.ToString()] = val.ToString();
            }
            return result;
        }
        private CellValue GetCellValueInternal(CellAddress addr)
        {
            if (!_cells.TryGetValue(addr, out var cell))
                return CellValue.Empty;

            if (cell.IsDirty)
            {
                // This shouldn't happen if we recalculate on set, but for lazy eval support:
                if (cell.Ast != null)
                    cell.CachedValue = _evaluator.Evaluate(cell.Ast);

                cell.IsDirty = false;
            }

            return cell.CachedValue;
        }

        private void Recalculate(CellAddress startNode)
        {
            var order = _graph.GetRecalculationOrder(startNode);
            foreach (var addr in order)
            {
                if (_cells.TryGetValue(addr, out var cell))
                {
                    if (cell.Ast != null)
                        cell.CachedValue = _evaluator.Evaluate(cell.Ast);
                    cell.IsDirty = false;
                }
            }
        }

        private void UpdateDependencies(CellAddress dependent, AstNode node)
        {
            // Traverse AST to find ReferenceNodes
            // Simple recursive visitor
            VisitDependencies(dependent, node);
        }

        private void VisitDependencies(CellAddress dependent, AstNode node)
        {
            if (node is CellReferenceNode refNode)
            {
                _graph.AddDependency(dependent, refNode.Address);
            }
            else if (node is BinaryOpNode b)
            {
                VisitDependencies(dependent, b.Left);
                VisitDependencies(dependent, b.Right);
            }
            else if (node is FunctionCallNode f)
            {
                foreach (var arg in f.Arguments)
                    VisitDependencies(dependent, arg);
            }
            else if (node is RangeNode r)
            {
                // Range implies dependence on all cells in range? 
                // Yes, simpler to treat as "bulk dependency" or expand.
                // For now, let's just ignore or expand if small.
                // Expanding is safest for correctness.
                for (int i = r.Start.Row; i <= r.End.Row; i++)
                    for (int j = r.Start.Column; j <= r.End.Column; j++)
                        _graph.AddDependency(dependent, new CellAddress(i, j));
            }
        }
    }
}
