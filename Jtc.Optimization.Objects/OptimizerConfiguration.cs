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
        /// The maximum genetic generations or iterations of maximizer
        /// </summary>
        public int Generations { get; set; } = 1000;

        /// <summary>
        /// Quit if fitness does not improve for this number of genetic generations
        /// </summary>
        public int StagnationGenerations { get; set; } = 10;

        /// <summary>
        /// Number of parallel backtests
        /// </summary>
        public int MaxThreads { get; set; } = 8;

        /// <summary>
        /// Override config.json setting with the class name of algorithm
        /// </summary>
        public string AlgorithmTypeName { get; set; }

        /// <summary>
        /// Full path to config.json
        /// </summary>
        public string ConfigPath { get; set; } = "../../../../Lean/Launcher/config.json";

        /// <summary>
        /// 1 or 2 point genetic crossover
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
        /// Type name of fitness function. Defaults to Sharpe Ratio fitness
        /// </summary>
        public string FitnessTypeName { get; set; } = "Jtc.Optimization.LeanOptimizer.OptimizerFitness";

        /// <summary>
        /// Override config.json setting for the folder containing historical trade and symbol data 
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
        /// Probability of genetic mutation
        /// </summary>
        public float MutationProbability { get; set; } = 0.1f;

        /// <summary>
        /// Probability of genetic crossover
        /// </summary>
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
        /// By default, the actual value of a gene is used for a single chromosome of the first generation. 
        /// Setting this to true will result in the entire first generation being populated with actal genes values.
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

        /// <summary>
        /// CSharp or Python. If specified will override config.json setting.
        /// </summary>
        public string AlgorithmLanguage { get; set; }
    }


}
