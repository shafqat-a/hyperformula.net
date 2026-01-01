using System;
using System.Collections.Generic;
using System.Text;

namespace HyperFormulaCS.Parsing
{
    public class Lexer
    {
        private readonly string _text;
        private int _position;

        public Lexer(string text)
        {
            _text = text ?? string.Empty;
            _position = 0;
        }

        private char Current => _position >= _text.Length ? '\0' : _text[_position];

        private void Next() => _position++;

        public IEnumerable<Token> Tokenize()
        {
            // If it starts with =, skip it as it's a formula marker
            if (_position == 0 && Current == '=')
                Next();

            while (_position < _text.Length)
            {
                if (char.IsWhiteSpace(Current))
                {
                    Next();
                }
                else if (char.IsDigit(Current) || Current == '.')
                {
                    yield return ReadNumber();
                }
                else if (char.IsLetter(Current))
                {
                    yield return ReadIdentifier();
                }
                else if (Current == '"')
                {
                    yield return ReadString();
                }
                else
                {
                    yield return ReadSymbol();
                }
            }

            yield return new Token(TokenType.EOF, "", _position);
        }

        private Token ReadNumber()
        {
            int start = _position;
            bool hasDot = false;

            while (char.IsDigit(Current) || (Current == '.' && !hasDot))
            {
                if (Current == '.') hasDot = true;
                Next();
            }

            return new Token(TokenType.Number, _text.Substring(start, _position - start), start);
        }

        private Token ReadIdentifier()
        {
            int start = _position;
            // Allow letters and numbers in identifiers (e.g. A1, SUM)
            while (char.IsLetterOrDigit(Current))
            {
                Next();
            }
            return new Token(TokenType.Identifier, _text.Substring(start, _position - start), start);
        }

        private Token ReadString()
        {
            int start = _position;
            Next(); // Skip opening quote
            var sb = new StringBuilder();

            while (Current != '\0' && Current != '"')
            {
                sb.Append(Current);
                Next();
            }

            if (Current == '"') Next(); // Skip closing quote

            return new Token(TokenType.String, sb.ToString(), start);
        }

        private Token ReadSymbol()
        {
            var start = _position;
            var type = TokenType.Unknown;
            var val = Current.ToString();

            switch (Current)
            {
                case '+': type = TokenType.Plus; break;
                case '-': type = TokenType.Minus; break;
                case '*': type = TokenType.Asterisk; break;
                case '/': type = TokenType.Slash; break;
                case '(': type = TokenType.LParen; break;
                case ')': type = TokenType.RParen; break;
                case ',': type = TokenType.Comma; break;
                case ':': type = TokenType.Colon; break;
                case '=': type = TokenType.Equals; break;
                case '<':
                    if (Peek() == '=') { Next(); val += "="; type = TokenType.LessThanOrEqual; }
                    else if (Peek() == '>') { Next(); val += ">"; type = TokenType.NotEquals; }
                    else type = TokenType.LessThan;
                    break;
                case '>':
                    if (Peek() == '=') { Next(); val += "="; type = TokenType.GreaterThanOrEqual; }
                    else type = TokenType.GreaterThan;
                    break;
            }

            Next();
            return new Token(type, val, start);
        }

        private char Peek() => _position + 1 < _text.Length ? _text[_position + 1] : '\0';
    }
}
