using GeneticSharp.Domain.Chromosomes;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer.Tests
{
    [TestFixture()]
    public class NFoldCrossSharpeMaximizerTests
    {

        List<string> _actualStartDate;
        List<string> _actualEndDate;

        [TestCase("2001-01-01", "2002-12-31", 2, new[] { "2001-01-01 00:00:00", "2002-01-01 00:00:00" }, new[] { "2001-12-31 23:59:59", "2002-12-31 23:59:59" })]
        [TestCase("2001-01-01", "2001-01-01", 2, new[] { "2001-01-01 00:00:00" }, new[] { "2001-01-01 23:59:59" })]
        [TestCase("2001-01-01", "2001-01-03", 3, new[] { "2001-01-01 00:00:00", "2001-01-02 00:00:00", "2001-01-03 00:00:00" },
            new[] { "2001-01-01 23:59:59", "2001-01-02 23:59:59", "2001-01-03 23:59:59" })]
        public void GetScoreTest(DateTime startDate, DateTime endDate, int folds, string[] expectedStartDate, string[] expectedEndDate)
        {
            _actualStartDate = new List<string>();
            _actualEndDate = new List<string>();

            var unit = new Mock<NFoldCrossSharpeMaximizer>(new OptimizerConfiguration
            {
                StartDate = startDate,
                EndDate = endDate,
                FitnessTypeName = "Optimization.OptimizerFitness",
                EnableFitnessFilter = true,
                Fitness = new FitnessConfiguration { Folds = folds }
            },
            new FitnessFilter())
            { CallBase = true };

            unit.Setup(x => x.RunAlgorithm(It.IsAny<Dictionary<string, object>>(), It.IsAny<IOptimizerConfiguration>())).Returns<Dictionary<string, object>, IOptimizerConfiguration>((l, c) =>
            {
                _actualStartDate.Add(ConvertDate(c.StartDate));
                _actualEndDate.Add(ConvertDate(c.EndDate));

                return new Dictionary<string, decimal> { { "SharpeRatio", 1 }, { "CompoundingAnnualReturn", 1 }, { "TotalNumberOfTrades", 1 },
                    { "LossRate", 0.1m } };
            });

            var actual = unit.Object.GetScore(new Dictionary<string, object>(), unit.Object.Config);

            for (int i = 0; i < expectedStartDate.Count(); i++)
            {
                Assert.AreEqual(expectedStartDate[i], _actualStartDate.ElementAt(i));
                Assert.AreEqual(expectedEndDate[i], _actualEndDate.ElementAt(i));
            }
            Assert.AreEqual(1, actual["SharpeRatio"]);
        }

        private string ConvertDate(object date)
        {
            return ((DateTime)date).ToString("yyyy-MM-dd HH:mm:ss");
        }

    }
}