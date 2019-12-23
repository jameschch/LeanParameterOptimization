using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient.Models
{

    public class GeneConfiguration
    {
        /// <summary>
        /// The unique key of the gene
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// The minimum value for a value
        /// </summary>
        [Required]
        public double? Min { get; set; }

        /// <summary>
        /// The maximum value for a value
        /// </summary>
        [Required]
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
        /// When true, will randomly select a value between 0 to 10946 from the Fibonacci sequence instead of generating random values
        /// </summary>
        /// <remarks></remarks>
        public bool Fibonacci { get; set; }

    }
}
