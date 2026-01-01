using System.Collections.Generic;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Ast
{
    public abstract class AstNode { }

    public class NumberNode : AstNode
    {
        public double Value { get; }
        public NumberNode(double value) => Value = value;
    }

    public class StringNode : AstNode
    {
        public string Value { get; }
        public StringNode(string value) => Value = value;
    }

    public class UnaryOpNode : AstNode
    {
        public AstNode Operand { get; }
        public string Op { get; }
        public UnaryOpNode(string op, AstNode operand)
        {
            Op = op;
            Operand = operand;
        }
    }

    public class BinaryOpNode : AstNode
    {
        public AstNode Left { get; }
        public string Op { get; }
        public AstNode Right { get; }

        public BinaryOpNode(AstNode left, string op, AstNode right)
        {
            Left = left;
            Op = op;
            Right = right;
        }
    }

    public class CellReferenceNode : AstNode
    {
        public CellAddress Address { get; }
        public CellReferenceNode(CellAddress address) => Address = address;
    }

    public class RangeNode : AstNode
    {
        public CellAddress Start { get; }
        public CellAddress End { get; }
        public RangeNode(CellAddress start, CellAddress end)
        {
            Start = start;
            End = end;
        }
    }

    public class FunctionCallNode : AstNode
    {
        public string FunctionName { get; }
        public List<AstNode> Arguments { get; }
        public FunctionCallNode(string name, List<AstNode> args)
        {
            FunctionName = name;
            Arguments = args;
        }
    }
}
