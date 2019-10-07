using Jtc.Optimization.Objects.Interfaces;

namespace Jtc.Optimization
{
    public interface IOptimizerManager
    {
        void Initialize(IOptimizerConfiguration config, OptimizerFitness fitness);
        void Start();
    }
}