using Xunit;
using HyperFormulaCS.Calculation;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Tests
{
    public class FunctionTests
    {
        [Fact]
        public void TestMathFunctions()
        {
            var engine = new Engine();

            engine.SetCell("A1", "=ABS(-10)");
            Assert.Equal(10, ((NumberValue)engine.GetCellValue("A1")).Value);

            engine.SetCell("B1", "=SQRT(16)");
            Assert.Equal(4, ((NumberValue)engine.GetCellValue("B1")).Value);

            engine.SetCell("C1", "=POWER(2, 3)");
            Assert.Equal(8, ((NumberValue)engine.GetCellValue("C1")).Value);

            engine.SetCell("D1", "=ROUND(3.14159, 2)");
            Assert.Equal(3.14, ((NumberValue)engine.GetCellValue("D1")).Value);
        }

        [Fact]
        public void TestLogicalFunctions()
        {
            var engine = new Engine();

            engine.SetCell("A1", "=AND(TRUE, TRUE)");
            Assert.True(((BooleanValue)engine.GetCellValue("A1")).Value);

            engine.SetCell("A2", "=AND(TRUE, FALSE)");
            Assert.False(((BooleanValue)engine.GetCellValue("A2")).Value);

            engine.SetCell("B1", "=OR(FALSE, TRUE)");
            Assert.True(((BooleanValue)engine.GetCellValue("B1")).Value);

            engine.SetCell("C1", "=NOT(TRUE)");
            Assert.False(((BooleanValue)engine.GetCellValue("C1")).Value);
        }

        [Fact]
        public void TestIfFunction()
        {
            var engine = new Engine();

            engine.SetCell("A1", "=IF(TRUE, 1, 2)");
            Assert.Equal(1, ((NumberValue)engine.GetCellValue("A1")).Value);

            engine.SetCell("B1", "=IF(FALSE, 1, 2)");
            Assert.Equal(2, ((NumberValue)engine.GetCellValue("B1")).Value);
        }

        [Fact]
        public void TestNestedIf()
        {
            var engine = new Engine();
            engine.SetCell("A1", "10");
            engine.SetCell("B1", "=IF(A1 > 5, \"Big\", \"Small\")");

            Assert.Equal("Big", ((StringValue)engine.GetCellValue("B1")).Value);

            engine.SetCell("A1", "3");
            Assert.Equal("Small", ((StringValue)engine.GetCellValue("B1")).Value);
        }
    }
}
