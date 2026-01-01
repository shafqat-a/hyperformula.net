using Xunit;
using HyperFormulaCS.Calculation;
using HyperFormulaCS.Models;
using System;

namespace HyperFormulaCS.Tests
{
    public class FinancialFunctionTests
    {
        [Fact]
        public void TestPmt()
        {
            var engine = new Engine();
            // PMT(rate, nper, pv)
            // 8% annual rate, 10 months, 10000 loan -> Monthly PMT?
            // Usually rate is per period. 8%/12?
            // Let's use simple numbers. Rate 0.1, nper 2, pv 100.
            // PMT = (100 * 0.1 * 1.1^2) / (1.1^2 - 1) ...

            // Excel Example: PMT(8%/12, 10, 10000) = -1037.03
            engine.SetCell("A1", "=PMT(0.08/12, 10, 10000)");

            double res = ((NumberValue)engine.GetCellValue("A1")).Value;
            Assert.Equal(-1037.03, res, 2);
        }

        [Fact]
        public void TestFv()
        {
            var engine = new Engine();
            // FV(rate, nper, pmt, [pv], [type])
            // Save 100 every month for 1 year at 6% annual (0.5% monthly).
            // FV = 100 * ((1.005^12 - 1) / 0.005)
            // 100 * 12.335... = 1233.56

            // Note: PMT is usually specific sign (negative if out of pocket).
            // If I save -100 (out of pocket), I have +FV.
            engine.SetCell("A1", "=FV(0.06/12, 12, -100, 0, 0)");

            double res = ((NumberValue)engine.GetCellValue("A1")).Value;
            Assert.Equal(1233.56, res, 2);
        }

        [Fact]
        public void TestPv()
        {
            var engine = new Engine();
            // PV(rate, nper, pmt, [fv], [type])
            // Loan pays 500 per month for 3 years, leaving 0 balance (FV=0). Rate 8%.
            // PV(0.08/12, 3*12, -500, 0)

            // Note: If PMT is -500 (money I pay), PV is + (loan I get).
            engine.SetCell("A1", "=PV(0.08/12, 36, -500, 0)");
            // Approx 15918 -> Excel 15,955.90
            double res = ((NumberValue)engine.GetCellValue("A1")).Value;
            Assert.Equal(15955.90, res, 2);
        }

        [Fact]
        public void TestNpv()
        {
            var engine = new Engine();
            // NPV(0.1, -100, 50, 60)
            // -100/1.1 + 50/1.21 + 60/1.331
            // -90.90 + 41.32 + 45.08 = -4.5

            engine.SetCell("A1", "=NPV(0.1, -100, 50, 60)");
            double res = ((NumberValue)engine.GetCellValue("A1")).Value;
            Assert.Equal(-4.50, res, 1);
        }
    }
}
