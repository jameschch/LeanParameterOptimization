using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;

namespace Jtc.Optimization.LeanOptimizer.Base
{
    public interface IRunner : IDisposable
    {
        Dictionary<string, decimal> Run(Dictionary<string, object> items, IOptimizerConfiguration config);
    }
}