using GeneticSharp.Domain.Fitnesses;
using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.LeanOptimizer
{
    public interface ISharpeMaximizerFactory
    {

        SharpeMaximizer Create(IOptimizerConfiguration config, IFitnessFilter filter);

    }
}
