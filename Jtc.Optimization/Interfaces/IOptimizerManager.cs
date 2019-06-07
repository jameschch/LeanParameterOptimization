namespace Optimization
{
    public interface IOptimizerManager
    {
        void Initialize(IOptimizerConfiguration config, OptimizerFitness fitness);
        void Start();
    }
}