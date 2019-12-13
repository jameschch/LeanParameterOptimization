using Jtc.Optimization.Objects.Interfaces;

namespace Jtc.Optimization.LeanOptimizer
{
    public interface IOptimizerManager
    {
        void Initialize(IOptimizerConfiguration config, OptimizerFitness fitness);
        void Start();
    }
}