using System;

namespace Optimization
{
    public interface IOptimizerConfiguration
    {
        GeneConfiguration[] Genes { get; set; }
        string AlgorithmLocation { get; set; }
        string AlgorithmTypeName { get; set; }
        string ConfigPath { get; set; }
        int Generations { get; set; }
        int MaxThreads { get; set; }
        bool OnePointCrossover { get; set; }
        int PopulationSize { get; set; }
        int PopulationSizeMaximum { get; set; }
        int StagnationGenerations { get; set; }
        bool IncludeNegativeReturn { get; set; }
        string FitnessTypeName { get; set; }
        string DataFolder { get; set; }
        FitnessConfiguration Fitness { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        float MutationProbability { get; set; }
        float CrossoverProbability { get; set; }
        int MinimumTrades { get; set; }
        bool EnableFitnessFilter { get; set; }
        bool UseActualGenesForWholeGeneration { get; set; }
        string TransactionLog { get; set; }
        bool EnableRunningDuplicateParameters { get; set; }
        bool UseSharedAppDomain { get; set; }
    }

}