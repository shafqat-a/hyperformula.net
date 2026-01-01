namespace HyperFormulaCS.Parsing
{
    public enum TokenType
    {
        Number,
        String,
        Identifier, // Function names or Cell refs (initially parsed as ids, then refined)
        Plus,
        Minus,
        Asterisk,
        Slash,
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        LParen,
        RParen,
        Comma,
        Colon, // For ranges A1:B2
        EOF,
        Unknown
    }

    public record Token(TokenType Type, string Value, int Position);
}
