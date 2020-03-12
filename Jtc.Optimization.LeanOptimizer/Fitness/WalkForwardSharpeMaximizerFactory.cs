using Jtc.Optimization.Objects.Interfaces;

namespace Jtc.Optimization.LeanOptimizer.Fitness
{
    class WalkForwardSharpeMaximizerFactory : IWalkForwardSharpeMaximizerFactory
    {
        public IWalkForwardSharpeMaximizer Create(IOptimizerConfiguration config, IFitnessFilter filter)
        {
            return new WalkForwardSharpeMaximizer(config, filter);
        }
    }
}
