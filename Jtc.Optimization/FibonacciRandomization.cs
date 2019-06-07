using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization
{

    /// <summary>
    /// Generates a random Fibonacci number within the range of 0 - 10946
    /// </summary>
    /// <remarks>This can supply typical indicator period values with the intention of reducing parameter over-fitting</remarks>
    public class FibonacciRandomization : BasicRandomization
    {

        private static int[] _fibonacci = { 0, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946 };
        private static readonly string _message = "Fibonacci range supported between 0 and 10946.";

        public override int GetInt(int min, int max)
        {

            var within = _fibonacci.Where(f => f >= min && f <= max);

            if (!within.Any())
            {
                throw new ArgumentException(_message);
            }

            var selection = within.ElementAt(base.GetInt(0, within.Count()));
            return selection;
        }

        public override int[] GetInts(int length, int min, int max)
        {
            var result = new int[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = GetInt(min, max);
            }

            return result;
        }

    }
}
