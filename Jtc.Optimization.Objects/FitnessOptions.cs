using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.Optimization.Objects
{
    public class FitnessOptions
    {

        private static FitnessDefinition[] Definition = new[] {
            new FitnessDefinition { Name = "AdaptiveSharpeRatioFitness", IsConfigurable = false, IsGenetic = true },
            new FitnessDefinition { Name = "CompoundingAnnualReturnFitness", IsConfigurable = false, IsGenetic = true },
            new FitnessDefinition { Name = "ConfiguredFitness", IsConfigurable = true, IsGenetic = true },
            new FitnessDefinition { Name = "DeflatedSharpeRatioFitness", IsConfigurable = false, IsGenetic = true },
            new FitnessDefinition { Name = "DualPeriodSharpeFitness", IsConfigurable = false, IsGenetic = true },
            new FitnessDefinition { Name = "NestedCrossSharpeMaximizer", IsConfigurable = true, IsGenetic = false },
            new FitnessDefinition { Name = "NFoldCrossReturnMaximizer", IsConfigurable = true, IsGenetic = false },
            new FitnessDefinition { Name = "NFoldCrossSharpeMaximizer", IsConfigurable = true, IsGenetic = false },
            new FitnessDefinition { Name = "OptimizerFitness", IsConfigurable = false, IsGenetic = true },
            new FitnessDefinition { Name = "SharpeMaximizer", IsConfigurable = true, IsGenetic = false }
        };

        public static string[] Name { get;  } = Definition.Select(d => d.Name).ToArray();
        public static string[] Configurable { get; } = Definition.Where(d => d.IsConfigurable).Select(d => d.Name).ToArray();
        public static string[] Genetic { get; } = Definition.Where(d => d.IsGenetic).Select(d => d.Name).ToArray();

        private class FitnessDefinition
        {

            public string Name { get; set; }
            public bool IsConfigurable { get; set; }
            public bool IsGenetic { get; set; }

        }

    }
}
