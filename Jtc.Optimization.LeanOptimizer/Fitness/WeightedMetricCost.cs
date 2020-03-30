using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using System;
using System.Linq;

namespace Jtc.Optimization.LeanOptimizer
{
    public class WeightedMetricCost
    {

        public static double Calculate(double[] sharpeRatio, double[][] parameters)
        {
            var normalizedSharpe = sharpeRatio.Select(s => Math.Round(Normalize(s), 3)).Average();

            var matrix = Matrix<double>.Build.DenseOfColumns(parameters);

            var meanDeviation = matrix.EnumerateRows().Select(s => Normalize(Statistics.PopulationStandardDeviation(s))).Average();

            return -meanDeviation + normalizedSharpe;
        }

        /// <summary>
        /// Normalize to +-4
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static double Normalize(double value)
        {
            return Math.Max(Math.Min(value, 4.0), -4.0);
        }

    }
}
