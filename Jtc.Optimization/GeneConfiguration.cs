using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization
{

    [JsonConverter(typeof(GeneConverter))]
    [Serializable]
    public class GeneConfiguration
    {
        /// <summary>
        /// The unique key of the gene
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The minimum value for a decimal value
        /// </summary>
        public decimal? MinDecimal { get; set; }

        /// <summary>
        /// The maximum value for a decimal value
        /// </summary>
        public decimal? MaxDecimal { get; set; }

        /// <summary>
        /// The minimum value for an int value
        /// </summary>
        public int? MinInt { get; set; }

        /// <summary>
        /// The maximum value for an int value
        /// </summary>
        public int? MaxInt { get; set; }

        /// <summary>
        /// The decimal precision (rounding) for gene values
        /// </summary>
        public int? Precision { get; set; }

        /// <summary>
        /// The non-random starting value of an int
        /// </summary>
        public int? ActualInt { get; set; }

        /// <summary>
        /// The non-random starting value of a decimal
        /// </summary>
        public decimal? ActualDecimal { get; set; }

        /// <summary>
        /// When true, will randomly select a value between 0 to 10946 from the Fibonacci sequence instead of generating random values
        /// </summary>
        /// <remarks></remarks>
        public bool Fibonacci { get; set; }

    }
}
