using Jtc.Optimization.Objects.Interfaces;

namespace Jtc.Optimization.LeanOptimizer.Fitness
{
    public interface IWalkForwardSharpeMaximizerFactory
    {
        IWalkForwardSharpeMaximizer Create(IOptimizerConfiguration config, IFitnessFilter filter);
    }
}