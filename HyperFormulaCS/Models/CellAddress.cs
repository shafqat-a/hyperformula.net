using System;
using System.Text.RegularExpressions;

namespace HyperFormulaCS.Models
{
    public readonly struct CellAddress : IEquatable<CellAddress>
    {
        public int Row { get; }
        public int Column { get; }

        public CellAddress(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return $"{GetColumnName(Column)}{Row + 1}";
        }

        public static CellAddress Parse(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty/null", nameof(address));

            var match = Regex.Match(address, @"^([A-Za-z]+)([0-9]+)$");
            if (!match.Success)
                throw new FormatException($"Invalid cell address format: {address}");

            string colPart = match.Groups[1].Value.ToUpperInvariant();
            int rowPart = int.Parse(match.Groups[2].Value);

            int colIndex = 0;
            int factor = 1;
            for (int i = colPart.Length - 1; i >= 0; i--)
            {
                colIndex += (colPart[i] - 'A' + 1) * factor;
                factor *= 26;
            }

            return new CellAddress(rowPart - 1, colIndex - 1);
        }
        
        // Helper to convert 0-based column index to "A", "AA", etc.
        private static string GetColumnName(int columnIndex)
        {
            string dividend = string.Empty;
            int modulo;

            columnIndex++; 

            while (columnIndex > 0)
            {
                modulo = (columnIndex - 1) % 26;
                dividend = Convert.ToChar(65 + modulo) + dividend;
                columnIndex = (int)((columnIndex - modulo) / 26);
            } 

            return dividend;
        }

        public bool Equals(CellAddress other) => Row == other.Row && Column == other.Column;
        public override bool Equals(object? obj) => obj is CellAddress other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Row, Column);
        public static bool operator ==(CellAddress left, CellAddress right) => left.Equals(right);
        public static bool operator !=(CellAddress left, CellAddress right) => !left.Equals(right);
    }
}
