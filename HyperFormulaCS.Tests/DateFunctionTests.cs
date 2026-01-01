using Xunit;
using HyperFormulaCS.Calculation;
using HyperFormulaCS.Models;
using System;

namespace HyperFormulaCS.Tests
{
    public class DateFunctionTests
    {
        [Fact]
        public void TestDate()
        {
            var engine = new Engine();
            engine.SetCell("A1", "=DATE(2023, 1, 1)");
            var res = ((NumberValue)engine.GetCellValue("A1")).Value;
            // 2023-01-01 -> approx 44927
            // Check approximate or exact logic
            var date = new DateTime(1899, 12, 30).AddDays(res);
            Assert.Equal(2023, date.Year);
            Assert.Equal(1, date.Month);
            Assert.Equal(1, date.Day);
        }

        [Fact]
        public void TestYearMonthDay()
        {
            var engine = new Engine();
            engine.SetCell("A1", "=DATE(2023, 5, 20)");
            engine.SetCell("B1", "=YEAR(A1)");
            engine.SetCell("B2", "=MONTH(A1)");
            engine.SetCell("B3", "=DAY(A1)");

            Assert.Equal(2023, ((NumberValue)engine.GetCellValue("B1")).Value);
            Assert.Equal(5, ((NumberValue)engine.GetCellValue("B2")).Value);
            Assert.Equal(20, ((NumberValue)engine.GetCellValue("B3")).Value);
        }

        [Fact]
        public void TestTime()
        {
            var engine = new Engine();
            engine.SetCell("A1", "=TIME(12, 30, 0)"); // 0.5208333...
            // 0.5 is 12:00. 12:30 is 12.5/24 
            double expected = 12.5 / 24.0;
            Assert.Equal(expected, ((NumberValue)engine.GetCellValue("A1")).Value, 5);
        }

        [Fact]
        public void TestHourMinuteSecond()
        {
            var engine = new Engine();
            engine.SetCell("A1", "=TIME(14, 45, 30)");
            engine.SetCell("B1", "=HOUR(A1)");
            engine.SetCell("B2", "=MINUTE(A1)");
            engine.SetCell("B3", "=SECOND(A1)");

            Assert.Equal(14, ((NumberValue)engine.GetCellValue("B1")).Value);
            Assert.Equal(45, ((NumberValue)engine.GetCellValue("B2")).Value);
            Assert.Equal(30, ((NumberValue)engine.GetCellValue("B3")).Value);
        }
    }
}
