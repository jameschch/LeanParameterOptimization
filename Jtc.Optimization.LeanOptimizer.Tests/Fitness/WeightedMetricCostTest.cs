using NUnit.Framework;
using System;

namespace Jtc.Optimization.LeanOptimizer.Tests.Fitness
{
    public class WeightedMetricCostTest
    {

        double[][] GetData(int index)
        {
            switch (index)
            {
                case 1:
                    return new double[][] { new[] { 10.0, 0.1 }, new[] { 10.0, 0.1 } };
                case 2:
                    return new double[][] { new[] { 10, 0.05 }, new[] { 5, 0.1 } };
                case 3:
                    return new double[][] { new[] { 1.1, 1.0 }, new[] { 1.0, 1.1 } };
                case 4:
                    return new double[][] { new[] { 10000, 0.01 }, new[] { 0.01, 10000 } };
            }

            throw new ArgumentException();

        }

        [TestCase(new[] { 1.0, 1.0 }, 1, 1)]
        [TestCase(new[] { 1.0, 1.0 }, 2, -0.26)]
        [TestCase(new[] { 1.0, 1.0 }, 3, 0.95)]
        [TestCase(new[] { 1.0, 0 }, 1, 0.5)]
        [TestCase(new[] { 1.0, 0 }, 3, 0.45)]
        [TestCase(new[] { 4.1, 4.1 }, 1, 4)]
        [TestCase(new[] { -4.1, -4.1 }, 1, -4)]
        [TestCase(new[] { 1.0, 1.0 }, 4, -3)]
        public void CalculateTest(double[] sharpeRatio, int dataIndex, double expected)
        {
            var data = GetData(dataIndex);

            var actual = WeightedMetricCost.Calculate(sharpeRatio, data);

            Assert.AreEqual(expected, actual, 0.01);
        }
    }
}