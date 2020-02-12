using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Jtc.Optimization.LeanOptimizer.Base;
using Jtc.Optimization.Objects.Interfaces;

namespace Jtc.Optimization.LeanOptimizer
{

    public class SingleAppDomainManager : BaseAppDomainManager<SingleAppDomainManager>
    {

        public SingleAppDomainManager() : base()
        {
            ResultMediator.SetResults(AppDomain.CurrentDomain, new Dictionary<string, Dictionary<string, decimal>>());
        }

        public override Dictionary<string, decimal> RunAlgorithm(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            var weak = new WeakReference<SingleRunner>(new SingleRunner());

            weak.TryGetTarget(out var rc);
            var result = rc.Run(list, config);
            rc = null;
            return result;
        }

        public new static Dictionary<string, Dictionary<string, decimal>> GetResults()
        {
            return ResultMediator.GetData<Dictionary<string, Dictionary<string, decimal>>>(AppDomain.CurrentDomain, "Results");
        }

        protected override IRunner CreateRunnerInAppDomain(ref AppDomain ad)
        {
            throw new NotSupportedException();
        }
    }

}
