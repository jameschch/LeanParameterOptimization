using Jtc.Optimization.Objects;
using NUnit.Framework;
using System;
using System.Linq;
using static DeepEqual.Syntax.ObjectExtensions;

namespace Jtc.Optimization.LeanOptimizer.Tests
{
    [TestFixture()]
    public class WalkForwardPeriodCalculatorTests
    {
        [TestCase("2001-01-01", "2001-01-04", 2,
            new[] {
            "2001-01-01", "2001-01-02", "2001-01-03", "2001-01-04",
            "2001-01-02", "2001-01-03", "2001-01-04", "2001-01-04"
            }, Description = "4 days, 2 folds. The minimum")]
        [TestCase("2001-01-01", "2001-01-06", 2,
            new[] {
            "2001-01-01", "2001-01-03", "2001-01-04", "2001-01-05",
            "2001-01-02", "2001-01-04", "2001-01-05", "2001-01-06"
            }, Description = "6 days, 2 folds")]
        [TestCase("2001-01-01", "2001-01-06", 3,
            new[] {
            "2001-01-01", "2001-01-02", "2001-01-03", "2001-01-04",
            "2001-01-02", "2001-01-03", "2001-01-04", "2001-01-05",
            "2001-01-03", "2001-01-04", "2001-01-05", "2001-01-06"
            }, Description = "6 days, 3 folds")]
        [TestCase("2001-01-01", "2002-07-02", 2,
            new[] {
            "2001-01-01", "2001-10-01", "2001-10-02", "2002-02-16",
            "2001-05-18", "2002-02-15", "2002-02-16", "2002-07-02"
            }, Description = "9 months, 2 folds")]
        public void CalculateTest(DateTime startDate, DateTime endDate, int folds, string[] expectedDate)
        {
            var unit = new WalkForwardPeriodCalculator();

            var actual = unit.Calculate(new OptimizerConfiguration
            {
                StartDate = startDate,
                EndDate = endDate,
                Fitness = new FitnessConfiguration { Folds = folds }
            });

            Assert.AreEqual(folds, actual.Count);

            expectedDate.Select(s => DateTime.Parse(s)).ShouldDeepEqual(actual.SelectMany(s => s.Value));
        }

        private string ConvertDate(object date)
        {
            return ((DateTime)date).ToString("yyyy-MM-dd HH:mm:ss");
        }

    }
}