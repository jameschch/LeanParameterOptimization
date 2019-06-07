using System.Collections.Generic;

namespace Optimization
{
    public interface IRunner
    {
        Dictionary<string, decimal> Run(Dictionary<string, object> items, IOptimizerConfiguration config);
    }
}