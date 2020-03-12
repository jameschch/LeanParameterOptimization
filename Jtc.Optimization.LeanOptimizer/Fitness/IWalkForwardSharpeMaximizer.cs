using GeneticSharp.Domain.Chromosomes;
using Jtc.Optimization.Objects;
using System.Collections.Generic;

namespace Jtc.Optimization.LeanOptimizer.Fitness
{
    public interface IWalkForwardSharpeMaximizer
    {
        List<Dictionary<string, object>> AllBest { get; }
        List<FitnessResult> AllScores { get; }

        double Evaluate(IChromosome chromosome);
    }
}