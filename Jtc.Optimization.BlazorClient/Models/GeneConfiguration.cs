using Jtc.Optimization.Objects.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient.Models
{

    [Serializable]
    public class GeneConfiguration
    {
        /// <summary>
        /// The unique key of the gene
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// The minimum value for a decimal value
        /// </summary>
        [Required]
        [JsonConverter(typeof(Jtc.Optimization.BlazorClient.Misc.DecimalJsonConverter))]
        public decimal? Min { get; set; }

        /// <summary>
        /// The maximum value for a decimal value
        /// </summary>
        [Required]
        [JsonConverter(typeof(Jtc.Optimization.BlazorClient.Misc.DecimalJsonConverter))]
        public decimal? Max { get; set; }

        /// <summary>
        /// The decimal precision (rounding) for gene values
        /// </summary>
        public int? Precision { get; set; }

        /// <summary>
        /// The non-random starting value of a decimal
        /// </summary>
        [JsonConverter(typeof(Jtc.Optimization.BlazorClient.Misc.DecimalJsonConverter))]
        public decimal? Actual { get; set; }

        /// <summary>
        /// When true, will randomly select a value between 0 to 10946 from the Fibonacci sequence instead of generating random values
        /// </summary>
        /// <remarks></remarks>
        public bool Fibonacci { get; set; }

    }
}
