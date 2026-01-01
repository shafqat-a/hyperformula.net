using Xunit;
using HyperFormulaCS.Calculation;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Tests
{
    public class LookupFunctionTests
    {
        [Fact]
        public void TestVLookup()
        {
            var engine = new Engine();
            // A | B
            // 1 | One
            // 2 | Two
            engine.SetCell("A1", "1"); engine.SetCell("B1", "One");
            engine.SetCell("A2", "2"); engine.SetCell("B2", "Two");

            // Look for 2, return column 2
            engine.SetCell("C1", "=VLOOKUP(2, A1:B2, 2, FALSE)");

            Assert.Equal("Two", ((StringValue)engine.GetCellValue("C1")).Value);

            // Not found
            engine.SetCell("C2", "=VLOOKUP(3, A1:B2, 2, FALSE)");
            Assert.IsType<ErrorValue>(engine.GetCellValue("C2"));
        }

        [Fact]
        public void TestMatch()
        {
            var engine = new Engine();
            engine.SetCell("A1", "Apple");
            engine.SetCell("A2", "Banana");
            engine.SetCell("A3", "Cherry");

            engine.SetCell("B1", "=MATCH(\"Banana\", A1:A3, 0)");
            Assert.Equal(2, ((NumberValue)engine.GetCellValue("B1")).Value);
        }

        [Fact]
        public void TestIndex()
        {
            var engine = new Engine();
            engine.SetCell("A1", "10"); engine.SetCell("B1", "20");
            engine.SetCell("A2", "30"); engine.SetCell("B2", "40");

            // Row 2, Col 2 -> B2 -> 40
            engine.SetCell("C1", "=INDEX(A1:B2, 2, 2)");
            Assert.Equal(40, ((NumberValue)engine.GetCellValue("C1")).Value);
        }
    }
}
