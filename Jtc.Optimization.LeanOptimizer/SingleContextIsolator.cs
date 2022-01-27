using Jtc.Optimization.LeanOptimizer.Base;
using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace Jtc.Optimization.LeanOptimizer
{

    public class SingleContextIsolator : BaseContextIsolator<SingleContextIsolator>
    {

        public SingleContextIsolator() : base()
        {
            ResultMediator.SetResults(AppDomain.CurrentDomain, new Dictionary<string, Dictionary<string, decimal>>());
        }

        public override Dictionary<string, decimal> RunAlgorithm(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            var context = AssemblyLoadContext.Default;
            using (context.EnterContextualReflection())
            {
                var runner = new SingleRunner();
                var result = runner.Run(list, config);
                runner = null;
                return result;
            }
        }

    }

}
