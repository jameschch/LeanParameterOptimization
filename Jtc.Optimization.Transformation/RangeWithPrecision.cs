using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.Optimization.Transformation
{
    public class RangeWithPrecision
    {


        public static IEnumerable<double> Range(double min, double max, int? precision)
        {
            if ((precision ?? 0) == 0)
            {
                return Enumerable.Range((int)min, (int)max - (int)min).Select(s => (double)s);
            }

            return Range(min, max, precision.Value);
        }

        private static IEnumerable<double> Range(double min, double max, int precision)
        {
            var stepSize = Math.Pow(0.1, precision);
            var decimalModifier = Math.Pow(10, precision);
            for (double i = min; i < max + stepSize; i += stepSize)
            {
                var item = Math.Truncate(i * decimalModifier) / decimalModifier;
                yield return item;
            }
        }

    }
}
