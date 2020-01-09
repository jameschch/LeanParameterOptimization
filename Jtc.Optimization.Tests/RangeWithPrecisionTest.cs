using DeepEqual.Syntax;
using Jtc.Optimization.Objects;
using Jtc.Optimization.OnlineOptimizer;
using Jtc.Optimization.Transformation;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Jtc.Optimization.Tests
{
    public class RangeWithPrecisionTest
    {

        [Fact]
        public void Given_min_max_precision_When_creating_range_Then_should_return_all_members()
        {
            var actual = RangeWithPrecision.Range(1, 2, 1).ToArray();
            var expected = new double[] { 1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0 };

            expected.ShouldDeepEqual(actual);
        }

        [Fact]
        public void Given_uneven_min_max_And_high_precision_When_creating_range_Then_should_return_all_members()
        {
            const double min = 0.056;
            const double max = 0.967;
            const int precision = 6;
            var actual = RangeWithPrecision.Range(min, max, precision).ToArray();

            Assert.Equal(CalculateLength(min, max, precision), actual.Length);
            Assert.Equal(min, actual[0]);
            Assert.Equal(0.056001, actual[1]);
            Assert.Equal(0.966999, actual.TakeLast(2).ElementAt(0));
            Assert.Equal(max, actual.Last());
        }

        private int CalculateLength(double min, double max, int precision)
        {
            return (int)(((decimal)max - (decimal)min) * (decimal)Math.Pow(10, precision)) + 1;
        }

    }
}
