using Xunit;
using HyperFormulaCS.Calculation;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Tests
{
    public class EngineTests
    {
        [Fact]
        public void TestBasicArithmetic()
        {
            var engine = new Engine();
            engine.SetCell("A1", "=1 + 2 * 3");
            var result = engine.GetCellValue("A1");

            Assert.IsType<NumberValue>(result);
            Assert.Equal(7, ((NumberValue)result).Value);
        }

        [Fact]
        public void TestCellReference()
        {
            var engine = new Engine();
            engine.SetCell("A1", "10");
            engine.SetCell("B1", "=A1 * 2");

            var result = engine.GetCellValue("B1");
            Assert.Equal(20, ((NumberValue)result).Value);
        }

        [Fact]
        public void TestDependencyChain()
        {
            var engine = new Engine();
            engine.SetCell("A1", "10");
            engine.SetCell("B1", "=A1 + 5");
            engine.SetCell("C1", "=B1 * 2"); // (10+5)*2 = 30

            Assert.Equal(30, ((NumberValue)engine.GetCellValue("C1")).Value);

            // Update A1, C1 should update
            engine.SetCell("A1", "20"); // B1->25, C1->50
            Assert.Equal(50, ((NumberValue)engine.GetCellValue("C1")).Value);
        }

        [Fact]
        public void TestSumFunction()
        {
            var engine = new Engine();
            engine.SetCell("A1", "10");
            engine.SetCell("A2", "20");
            engine.SetCell("A3", "=SUM(A1, A2)");

            Assert.Equal(30, ((NumberValue)engine.GetCellValue("A3")).Value);
        }

        [Fact]
        public void TestSumRange()
        {
            var engine = new Engine();
            engine.SetCell("A1", "1");
            engine.SetCell("A2", "2");
            engine.SetCell("B1", "3");
            engine.SetCell("B2", "4");
            engine.SetCell("C1", "=SUM(A1:B2)"); // 1+2+3+4 = 10

            Assert.Equal(10, ((NumberValue)engine.GetCellValue("C1")).Value);
        }

        [Fact]
        public void TestRecalculationWithRange()
        {
            var engine = new Engine();
            engine.SetCell("A1", "1");
            engine.SetCell("A2", "2");
            engine.SetCell("C1", "=SUM(A1:A2)"); // 3

            Assert.Equal(3, ((NumberValue)engine.GetCellValue("C1")).Value);

            engine.SetCell("A1", "5"); // 5+2=7
            Assert.Equal(7, ((NumberValue)engine.GetCellValue("C1")).Value);
        }
    }
}
