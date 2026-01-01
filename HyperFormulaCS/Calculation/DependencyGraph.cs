using System.Collections.Generic;
using System.Linq;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Calculation
{
    public class DependencyGraph
    {
        // Key depends on Values
        private readonly Dictionary<CellAddress, HashSet<CellAddress>> _dependencies = new();
        // Key is needed by Values (Reverse graph)
        private readonly Dictionary<CellAddress, HashSet<CellAddress>> _dependents = new();

        public void AddDependency(CellAddress dependent, CellAddress precedent)
        {
            if (!_dependencies.ContainsKey(dependent)) _dependencies[dependent] = new HashSet<CellAddress>();
            if (!_dependents.ContainsKey(precedent)) _dependents[precedent] = new HashSet<CellAddress>();

            _dependencies[dependent].Add(precedent);
            _dependents[precedent].Add(dependent);
        }

        public void RemoveDependencies(CellAddress dependent)
        {
            if (_dependencies.TryGetValue(dependent, out var precedents))
            {
                foreach (var prec in precedents)
                {
                    if (_dependents.ContainsKey(prec))
                    {
                        _dependents[prec].Remove(dependent);
                    }
                }
                _dependencies.Remove(dependent);
            }
        }

        public IEnumerable<CellAddress> GetDependents(CellAddress cell)
        {
            if (_dependents.TryGetValue(cell, out var deps))
                return deps;
            return Enumerable.Empty<CellAddress>();
        }

        public IEnumerable<CellAddress> GetDirectPrecedents(CellAddress cell)
        {
            if (_dependencies.TryGetValue(cell, out var precs))
                return precs;
            return Enumerable.Empty<CellAddress>();
        }

        // Returns topological sort order for recalculation
        public List<CellAddress> GetRecalculationOrder(CellAddress changedCell)
        {
            var visited = new HashSet<CellAddress>();
            var stack = new Stack<CellAddress>();
            var order = new List<CellAddress>();

            stack.Push(changedCell);

            // Simple DFS to find all affected nodes
            // Note: This needs a robust topo sort if we want correct calculation order 
            // but for simple cases, finding all reachable dependents is step 1.
            // For correct order: we really need to sort the subgraph of affected nodes.

            // Collect all transitive dependents
            var affected = new HashSet<CellAddress>();
            var queue = new Queue<CellAddress>();
            queue.Enqueue(changedCell);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (!affected.Contains(current))
                {
                    affected.Add(current);
                    foreach (var dep in GetDependents(current))
                    {
                        queue.Enqueue(dep);
                    }
                }
            }

            // Now Topological Sort on the affected subgraph
            var subgraphDeps = new Dictionary<CellAddress, int>();
            foreach (var node in affected) subgraphDeps[node] = 0;

            foreach (var node in affected)
            {
                foreach (var prec in GetDirectPrecedents(node))
                {
                    if (affected.Contains(prec))
                    {
                        subgraphDeps[node]++;
                    }
                }
            }

            var readyQueue = new Queue<CellAddress>();
            foreach (var kvp in subgraphDeps)
            {
                if (kvp.Value == 0) readyQueue.Enqueue(kvp.Key);
            }

            while (readyQueue.Count > 0)
            {
                var u = readyQueue.Dequeue();
                order.Add(u);

                foreach (var v in GetDependents(u))
                {
                    if (affected.Contains(v)) // Only consider nodes in subgraph
                    {
                        subgraphDeps[v]--;
                        if (subgraphDeps[v] == 0)
                            readyQueue.Enqueue(v);
                    }
                }
            }

            return order;
        }

        public bool HasCycle(CellAddress start, CellAddress end)
        {
            // Simple check: is 'start' accessible from 'end'? 
            // If we add end -> start dependency, and start is already a precedent of end, that's a cycle.
            // i.e., Does 'start' appear in Precedents(end)?
            // Wait, logic: if we add A -> B (A depends on B), we check if B depends on A.

            // Use DFS or BFS
            var visited = new HashSet<CellAddress>();
            var queue = new Queue<CellAddress>();
            queue.Enqueue(end);

            while (queue.Count > 0)
            {
                var curr = queue.Dequeue();
                if (curr.Equals(start)) return true; // Found path back to start

                if (visited.Add(curr))
                {
                    foreach (var next in GetDirectPrecedents(curr))
                    {
                        queue.Enqueue(next);
                    }
                }
            }
            return false;
        }
    }
}
