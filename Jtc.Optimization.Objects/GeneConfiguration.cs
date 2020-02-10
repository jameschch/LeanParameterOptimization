using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.Objects
{

    [Serializable]
    public class GeneConfiguration : IGeneConfiguration
    {
        /// <summary>
        /// The unique key of the gene
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The minimum value
        /// </summary>
        public double? Min { get; set; }

        /// <summary>
        /// The maximum value
        /// </summary>
        public double? Max { get; set; }

        /// <summary>
        /// The precision (rounding) for gene values
        /// </summary>
        public int? Precision { get; set; }

        /// <summary>
        /// The non-random starting value
        /// </summary>
        public double? Actual { get; set; }

        /// <summary>
        /// When true, will randomly select a value between 0 to 10946 from the Fibonacci sequence instead of generating random values in Genetic optimization
        /// </summary>
        /// <remarks></remarks>
        public bool Fibonacci { get; set; }

    }
}
