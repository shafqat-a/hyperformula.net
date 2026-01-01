using System;

namespace HyperFormulaCS.Models
{
    public abstract record CellValue
    {
        public static readonly CellValue Empty = new EmptyValue();
    }

    public record NumberValue(double Value) : CellValue
    {
        public override string ToString() => Value.ToString();
    }

    public record StringValue(string Value) : CellValue
    {
        public override string ToString() => Value;
    }

    public record BooleanValue(bool Value) : CellValue
    {
        public override string ToString() => Value.ToString().ToUpper();
    }

    public record ErrorValue(string Message) : CellValue
    {
        public static readonly ErrorValue Value = new ErrorValue("#VALUE!");
        public static readonly ErrorValue Div0 = new ErrorValue("#DIV/0!");
        public static readonly ErrorValue Ref = new ErrorValue("#REF!");
        public static readonly ErrorValue Name = new ErrorValue("#NAME?");
        public static readonly ErrorValue NA = new ErrorValue("#N/A");
        public static readonly ErrorValue Num = new ErrorValue("#NUM!");

        public override string ToString() => Message;
    }

    public record EmptyValue : CellValue
    {
        public override string ToString() => "";
    }
}
