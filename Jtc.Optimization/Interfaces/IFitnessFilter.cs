using System.Collections.Generic;

namespace Jtc.Optimization.LeanOptimizer
{
    public interface IFitnessFilter
    {
        bool IsSuccess(Dictionary<string, decimal> result, OptimizerFitness fitness);
    }
}