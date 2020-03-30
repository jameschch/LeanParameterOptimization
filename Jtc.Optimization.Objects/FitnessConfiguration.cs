using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Jtc.Optimization.Objects.Enums;

namespace Jtc.Optimization.Objects
{

    [Serializable]
    public class FitnessConfiguration : IFitnessConfiguration
    {

        /// <summary>
        /// Name of the fitness
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Field name for Lean result statistic
        /// </summary>
        public string ResultKey { get; set; }
        /// <summary>
        /// For ConfiguredFitenss, the scale factor of the fitness with a default value of 1. The maximum fitness value is 10000.
        /// </summary>
        public double? Scale { get; set; }
        /// <summary>
        /// For ConfiguredFitenss, the modifier function of the fitness with a default value of 1. A value of -1 will invert the optimization to minimize the algorithm statistic result.
        /// </summary>
        public double? Modifier { get; set; }
        /// <summary>
        /// Type of fitness/optimization algorithm
        /// </summary>
        public string OptimizerTypeName { get; set; } = OptimizerTypeOptions.Genetic.ToString();
        /// <summary>
        /// Number of cross validation folds
        /// </summary>
        public int Folds { get; set; }
        /// <summary>
        /// Type of fitness/optimization algorithm to use for Walk forward weighted metric fold
        /// </summary>
        public string FoldOptimizerTypeName { get; set; }
        /// <summary>
        /// Number of generations for each fold for Walk forward weighted metric fold
        /// </summary>
        public int? FoldGenerations { get; set; }

    }

}
