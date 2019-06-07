using System.Collections.Generic;

namespace Optimization
{
    public interface IFitnessFilter
    {
        bool IsSuccess(Dictionary<string, decimal> result, OptimizerFitness fitness);
    }
}