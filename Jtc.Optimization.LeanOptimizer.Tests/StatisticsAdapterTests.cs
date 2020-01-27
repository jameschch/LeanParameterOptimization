using NUnit.Framework;
using QuantConnect.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer.Tests
{
    [TestFixture()]
    public class StatisticsAdapterTests
    {

        [Test()]
        public void TransformTest()
        {
            var actual = StatisticsAdapter.Transform(new AlgorithmPerformance
            {
                PortfolioStatistics = new PortfolioStatistics
                {
                    Alpha = 1.23m
                },
                TradeStatistics = new TradeStatistics
                {
                    TotalNumberOfTrades = 0,
                    TotalFees = 0
                }
            },
            new Dictionary<string, string> { { "Total Trades", "45" }, { "Total Fees", "$1.23" } });

            Assert.AreEqual(1.23m, actual["Alpha"]);
            Assert.AreEqual(45m, actual["TotalNumberOfTrades"]);
            Assert.AreEqual(1.23m, actual["TotalFees"]);
        }

    }
}