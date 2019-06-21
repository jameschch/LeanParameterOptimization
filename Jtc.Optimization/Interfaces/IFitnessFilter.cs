using System.Collections.Generic;

namespace Jtc.Optimization
{
    public interface IFitnessFilter
    {
        bool IsSuccess(Dictionary<string, decimal> result, OptimizerFitness fitness);
    }
}