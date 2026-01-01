using System;
using System.Collections.Generic;
using System.Linq;
using HyperFormulaCS.Ast;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Parsing
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _position;

        public Parser(string text)
        {
            var lexer = new Lexer(text);
            _tokens = lexer.Tokenize().ToList();
            _position = 0;
        }

        private Token Current => _position < _tokens.Count ? _tokens[_position] : _tokens.Last();
        private void Next() => _position++;
        private bool Match(TokenType type)
        {
            if (Current.Type == type)
            {
                Next();
                return true;
            }
            return false;
        }

        public AstNode Parse()
        {
            return ParseComparison();
        }

        private AstNode ParseComparison()
        {
            var left = ParseAdditive();

            if (Current.Type == TokenType.Equals ||
                Current.Type == TokenType.NotEquals ||
                Current.Type == TokenType.GreaterThan ||
                Current.Type == TokenType.GreaterThanOrEqual ||
                Current.Type == TokenType.LessThan ||
                Current.Type == TokenType.LessThanOrEqual)
            {
                var op = Current.Value;
                Next();
                var right = ParseAdditive();
                left = new BinaryOpNode(left, op, right);
            }
            return left;
        }

        private AstNode ParseAdditive()
        {
            // Additive: + -
            var left = ParseTerm();

            while (Current.Type == TokenType.Plus || Current.Type == TokenType.Minus)
            {
                var op = Current.Value;
                Next();
                var right = ParseTerm();
                left = new BinaryOpNode(left, op, right);
            }
            return left;
        }

        private AstNode ParseTerm()
        {
            // Multiplicative: * /
            var left = ParseUnary();

            while (Current.Type == TokenType.Asterisk || Current.Type == TokenType.Slash)
            {
                var op = Current.Value;
                Next();
                var right = ParseUnary();
                left = new BinaryOpNode(left, op, right);
            }
            return left;
        }

        private AstNode ParseUnary()
        {
            if (Current.Type == TokenType.Plus || Current.Type == TokenType.Minus)
            {
                var op = Current.Value;
                Next();
                var right = ParseUnary();
                return new UnaryOpNode(op, right);
            }
            return ParseRange();
        }

        private AstNode ParseRange()
        {
            var left = ParsePrimary();

            if (Current.Type == TokenType.Colon)
            {
                Next();
                var right = ParsePrimary();

                if (left is CellReferenceNode lRef && right is CellReferenceNode rRef)
                {
                    return new RangeNode(lRef.Address, rRef.Address);
                }
                throw new ParseException("Range operator ':' requires cell references on both sides.");
            }

            return left;
        }

        private AstNode ParsePrimary()
        {
            if (Match(TokenType.LParen))
            {
                var expr = ParseComparison();
                if (!Match(TokenType.RParen))
                    throw new ParseException("Expected ')'");
                return expr;
            }

            if (Current.Type == TokenType.Number)
            {
                var val = double.Parse(Current.Value);
                Next();
                return new NumberNode(val);
            }

            if (Current.Type == TokenType.String)
            {
                var val = Current.Value;
                Next();
                return new StringNode(val);
            }

            if (Current.Type == TokenType.Identifier)
            {
                var name = Current.Value;
                Next();

                // Check if boolean literal
                if (string.Equals(name, "TRUE", StringComparison.OrdinalIgnoreCase))
                    return new FunctionCallNode("TRUE", new List<AstNode>());
                if (string.Equals(name, "FALSE", StringComparison.OrdinalIgnoreCase))
                    return new FunctionCallNode("FALSE", new List<AstNode>());

                // Check if function call
                if (Current.Type == TokenType.LParen)
                {
                    return ParseFunctionCall(name);
                }

                // Check if Cell Address
                try
                {
                    // This is a simplistic check, better to have TryParse or Regex check
                    var addr = CellAddress.Parse(name);
                    return new CellReferenceNode(addr);
                }
                catch
                {
                    throw new ParseException($"Unknown identifier '{name}'");
                }
            }

            throw new ParseException($"Unexpected token: {Current.Type} ({Current.Value})");
        }

        private AstNode ParseFunctionCall(string name)
        {
            Next(); // consume '('
            var args = new List<AstNode>();

            if (Current.Type != TokenType.RParen)
            {
                do
                {
                    args.Add(ParseComparison());
                } while (Match(TokenType.Comma));
            }

            if (!Match(TokenType.RParen))
                throw new ParseException("Expected ')' after function arguments.");

            return new FunctionCallNode(name.ToUpperInvariant(), args);
        }
    }

    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}
