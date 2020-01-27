using System;
using System.Threading.Tasks;

namespace SharpLearning.Optimization
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOptimizer
    {
        /// <summary>
        /// Returns the result which best minimises the provided function.
        /// </summary>
        /// <param name="functionToMinimize"></param>
        /// <returns></returns>
        Task<OptimizerResult> OptimizeBest(Func<double[], Task<OptimizerResult>> functionToMinimize);
        
        /// <summary>
        /// Returns all results ordered from best to worst (minimized). 
        /// </summary>
        /// <param name="functionToMinimize"></param>
        /// <returns></returns>
        Task<OptimizerResult[]> Optimize(Func<double[], Task<OptimizerResult>> functionToMinimize);
    }
}
