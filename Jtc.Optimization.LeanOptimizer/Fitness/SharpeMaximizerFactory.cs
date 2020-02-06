using GeneticSharp.Domain.Fitnesses;
using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.LeanOptimizer
{
    class SharpeMaximizerFactory : ISharpeMaximizerFactory
    {
        public SharpeMaximizer Create(IOptimizerConfiguration config, IFitnessFilter filter)
        {
            return new SharpeMaximizer(config, filter);
        }
    }
}
