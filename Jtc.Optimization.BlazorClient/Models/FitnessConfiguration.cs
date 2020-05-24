using System;
using System.ComponentModel.DataAnnotations;
using static Jtc.Optimization.Objects.Enums;
using Jtc.Optimization.Objects.Interfaces;

namespace Jtc.Optimization.BlazorClient.Models
{
    [Serializable]
    public class FitnessConfiguration : IFitnessConfiguration
    {

        [Required]
        public string Name { get; set; }
		
        [Required]
        public string ResultKey { get; set; }

        [Range(1, int.MaxValue)]
        public double? Scale { get; set; } = 1;

        public double? Modifier { get; set; } = 1;

        public string OptimizerTypeName { get; set; } = OptimizerTypeOptions.Genetic.ToString();

        [Range(1, int.MaxValue)]
        public int Folds { get; set; } = 1;

        public string FoldOptimizerTypeName { get; set; }

        public int? FoldGenerations { get; set; }

    }
}
