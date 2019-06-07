using GeneticSharp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Optimization.Enums;

namespace Optimization
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
        public int PopulationSize { get; set; } = 12;

        /// <summary>
        /// The maximum population
        /// </summary>
        public int PopulationSizeMaximum { get; set; } = 24;

        /// <summary>
        /// The maximum generations
        /// </summary>
        public int Generations { get; set; } = 1000;

        /// <summary>
        /// Quit if fitness does not improve for generations
        /// </summary>
        public int StagnationGenerations { get; set; } = 10;

        /// <summary>
        /// Number of parallel backtests
        /// </summary>
        public int MaxThreads { get; set; } = 8;

        /// <summary>
        /// Override config.json setting
        /// </summary>
        public string AlgorithmTypeName { get; set; }

        /// <summary>
        /// Full path to config.json
        /// </summary>
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
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Algorithm backtest end date
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Likeliness of mutation
        /// </summary>
        public float MutationProbability { get; set; } = GeneticAlgorithm.DefaultMutationProbability;

        /// <summary>
        /// Likeliness of crossover
        /// </summary>
        public float CrossoverProbability { get; set; } = GeneticAlgorithm.DefaultCrossoverProbability;

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
        public string Name { get; set; }
        /// <summary>
        /// Field name for Lean result statistic
        /// </summary>
        public string ResultKey { get; set; }
        /// <summary>
        /// The scale factor of the fitness with a default value of 1. The maximum fitness value is 10000.
        /// </summary>
        public double? Scale { get; set; }
        /// <summary>
        /// The modifier function of the fitness with a default value of 1. A value of -1 will invert the optimization to minimize the algorithm statistic result.
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
    }
}
