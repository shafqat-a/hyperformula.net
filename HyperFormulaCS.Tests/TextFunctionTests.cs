using Xunit;
using HyperFormulaCS.Calculation;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Tests
{
    public class TextFunctionTests
    {
        [Fact]
        public void TestConcatenate()
        {
            var engine = new Engine();
            engine.SetCell("A1", "Hello");
            engine.SetCell("B1", "World");
            engine.SetCell("C1", "=CONCATENATE(A1, \" \", B1)");
            Assert.Equal("Hello World", ((StringValue)engine.GetCellValue("C1")).Value);
        }

        [Fact]
        public void TestLen()
        {
            var engine = new Engine();
            engine.SetCell("A1", "abcde");
            engine.SetCell("B1", "=LEN(A1)");
            Assert.Equal(5, ((NumberValue)engine.GetCellValue("B1")).Value);
        }

        [Fact]
        public void TestLowerUpper()
        {
            var engine = new Engine();
            engine.SetCell("A1", "HeLLo");
            engine.SetCell("B1", "=LOWER(A1)");
            engine.SetCell("C1", "=UPPER(A1)");
            Assert.Equal("hello", ((StringValue)engine.GetCellValue("B1")).Value);
            Assert.Equal("HELLO", ((StringValue)engine.GetCellValue("C1")).Value);
        }

        [Fact]
        public void TestLeftRightMid()
        {
            var engine = new Engine();
            engine.SetCell("A1", "abcdef");

            engine.SetCell("B1", "=LEFT(A1, 2)");
            Assert.Equal("ab", ((StringValue)engine.GetCellValue("B1")).Value);

            engine.SetCell("C1", "=RIGHT(A1, 2)");
            Assert.Equal("ef", ((StringValue)engine.GetCellValue("C1")).Value);

            engine.SetCell("D1", "=MID(A1, 2, 3)"); // start at 2 (b), len 3 -> bcd
            Assert.Equal("bcd", ((StringValue)engine.GetCellValue("D1")).Value);
        }

        [Fact]
        public void TestTrim()
        {
            var engine = new Engine();
            engine.SetCell("A1", "  abc   ");
            engine.SetCell("B1", "=TRIM(A1)");
            Assert.Equal("abc", ((StringValue)engine.GetCellValue("B1")).Value);
        }

        [Fact]
        public void TestRept()
        {
            var engine = new Engine();
            engine.SetCell("A1", "ha");
            engine.SetCell("B1", "=REPT(A1, 3)");
            Assert.Equal("hahaha", ((StringValue)engine.GetCellValue("B1")).Value);
        }
    }
}
