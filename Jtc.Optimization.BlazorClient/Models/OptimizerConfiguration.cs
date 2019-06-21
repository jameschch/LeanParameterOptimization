using Jtc.Optimization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtc.Optimization.BlazorClient.Attributes;
using static Jtc.Optimization.Enums;

namespace Jtc.Optimization.BlazorClient.Models
{
    [Serializable]
    public class OptimizerConfiguration : IOptimizerConfiguration
    {

        /// <summary>
        /// The settings to generate gene values
        /// </summary>
        public GeneConfiguration[] Genes { get; set; }

        /// <summary>
        /// The initial size of the population
        /// </summary>
        [Range(2, int.MaxValue)]
        public int PopulationSize { get; set; } = 12;

        /// <summary>
        /// The maximum population
        /// </summary>
        [Range(2, int.MaxValue)]
        public int PopulationSizeMaximum { get; set; } = 24;

        /// <summary>
        /// The maximum generations
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Generations { get; set; } = 1000;

        /// <summary>
        /// Quit if fitness does not improve for generations
        /// </summary>
        [Range(1, int.MaxValue)]
        public int StagnationGenerations { get; set; } = 10;

        /// <summary>
        /// Number of parallel backtests
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxThreads { get; set; } = 8;

        /// <summary>
        /// Override config.json setting
        /// </summary>
        [Required]
        public string AlgorithmTypeName { get; set; }

        /// <summary>
        /// Full path to config.json
        /// </summary>
        [Required]
        public string ConfigPath { get; set; } = "../../../../Lean/Launcher/config.json";

        /// <summary>
        /// 1 or 2 point crossover
        /// </summary>
        public bool OnePointCrossover { get; set; } = false;

        /// <summary>
        /// Override config.json setting
        /// </summary>
        public string AlgorithmLocation { get; set; }

        /// <summary>
        /// By default results with negative Sharpe or CAR are ignored
        /// </summary>
        public bool IncludeNegativeReturn { get; set; }

        /// <summary>
        /// Type name of fitness function. Defaults to fitness based on Sharpe Ratio
        /// </summary>
        public string FitnessTypeName { get; set; } = "Optimization.OptimizerFitness";

        /// <summary>
        /// Override config.json setting
        /// </summary>
        public string DataFolder { get; set; }

        /// <summary>
        /// Settings for use with the ConfiguredFitness
        /// </summary>
        public FitnessConfiguration Fitness { get; set; }

        /// <summary>
        /// Algorithm backtest start date
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Algorithm backtest end date
        /// </summary>
        [GreaterThan("StartDate")]
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Likeliness of mutation
        /// </summary>
        [Range(0, 1)]
        public float MutationProbability { get; set; } = 0.1f;

        /// <summary>
        /// Likeliness of crossover
        /// </summary>
        [Range(0, 1)]
        public float CrossoverProbability { get; set; } = 0.75f;

        /// <summary>
        /// The minimum number of trades to consider the execution a non-failure
        /// </summary>
        public int MinimumTrades { get; set; }

        /// <summary>
        /// Enables the fitness filter that discards probable false positive executions
        /// </summary>
        public bool EnableFitnessFilter { get; set; }

        /// <summary>
        /// The actual settings for a gene by default are used for a single chromosome of the first generation. 
        /// Setting this to true will result in the entire first generation being populated with any actal genes specified.
        /// </summary>
        public bool UseActualGenesForWholeGeneration { get; set; }

        public string TransactionLog { get; set; }

        /// <summary>
        /// If true, will execute algorithms in a single AppDomain, allowing object instance sharing between iterations and generations.
        /// </summary>
        public bool UseSharedAppDomain { get; set; }

        /// <summary>
        /// If true, will always execute algorithm even if supplied parameters have previously been executed
        /// </summary>
        /// <remarks>May be used for non-deterministic execution results</remarks>
        public bool EnableRunningDuplicateParameters { get; set; }
    }

    [Serializable]
    public class FitnessConfiguration : IFitnessConfiguration
    {

        /// <summary>
        /// Name of the fitness
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Field name for Lean result statistic
        /// </summary>
        [Required]
        public string ResultKey { get; set; }
        /// <summary>
        /// The scale factor of the fitness with a default value of 1.
        /// </summary>
        [Range(1, int.MaxValue)]
        public double? Scale { get; set; } = 1;
        /// <summary>
        /// The modifier function of the fitness with a default value of 1. A value of -1 will invert the optimization to minimize the algorithm statistic result.
        /// </summary>
        public double? Modifier { get; set; } = 1;
        /// <summary>
        /// Type of fitness/optimization algorithm
        /// </summary>
        public string OptimizerTypeName { get; set; } = OptimizerTypeOptions.Genetic.ToString();
        /// <summary>
        /// Number of cross validation folds
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Folds { get; set; } = 1;
    }
}
