using Xunit;
using HyperFormulaCS.Calculation;
using HyperFormulaCS.Models;

namespace HyperFormulaCS.Tests
{
    public class StatisticalFunctionTests
    {
        [Fact]
        public void TestAverage()
        {
            var engine = new Engine();
            engine.SetCell("A1", "10");
            engine.SetCell("A2", "20");
            engine.SetCell("A3", "30");
            engine.SetCell("B1", "=AVERAGE(A1:A3)");
            Assert.Equal(20, ((NumberValue)engine.GetCellValue("B1")).Value);
        }

        [Fact]
        public void TestCount()
        {
            var engine = new Engine();
            engine.SetCell("A1", "10");
            engine.SetCell("A2", "text");
            engine.SetCell("A3", "30");
            engine.SetCell("B1", "=COUNT(A1:A3)"); // Should enable counting only numbers
            Assert.Equal(2, ((NumberValue)engine.GetCellValue("B1")).Value);
        }

        [Fact]
        public void TestCountA()
        {
            var engine = new Engine();
            engine.SetCell("A1", "10");
            engine.SetCell("A2", "text");
            engine.SetCell("A3", ""); // empty
            engine.SetCell("B1", "=COUNTA(A1:A3)");
            // A3 is empty literal "" -> actually StringValue or EmptyValue?
            // If SetCell("A3", "") -> usually EmptyValue if empty string.
            // Let's check SetCell behavior.
            Assert.Equal(2, ((NumberValue)engine.GetCellValue("B1")).Value);
        }

        [Fact]
        public void TestMaxMin()
        {
            var engine = new Engine();
            engine.SetCell("A1", "10");
            engine.SetCell("A2", "5");
            engine.SetCell("A3", "20");

            engine.SetCell("B1", "=MAX(A1:A3)");
            Assert.Equal(20, ((NumberValue)engine.GetCellValue("B1")).Value);

            engine.SetCell("B2", "=MIN(A1:A3)");
            Assert.Equal(5, ((NumberValue)engine.GetCellValue("B2")).Value);
        }
    }
}
