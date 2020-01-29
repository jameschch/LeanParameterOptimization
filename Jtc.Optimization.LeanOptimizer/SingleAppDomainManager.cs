using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Jtc.Optimization.Objects.Interfaces;

namespace Jtc.Optimization.LeanOptimizer
{

    public class SingleAppDomainManager : BaseAppDomainManager<SingleAppDomainManager>
    {

        public SingleAppDomainManager() : base()
        {
            ResultMediator.SetResults(AppDomain.CurrentDomain, new Dictionary<string, Dictionary<string, decimal>>());
        }

        static SingleRunner CreateRunnerInAppDomain()
        {
            return new SingleRunner();
        }

        public override Dictionary<string, decimal> RunAlgorithm(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            var rc = CreateRunnerInAppDomain();

            var result = rc.Run(list, config);

            return result;
        }

        public new static Dictionary<string, Dictionary<string, decimal>> GetResults()
        {
            return  ResultMediator.GetData<Dictionary<string, Dictionary<string, decimal>>>(AppDomain.CurrentDomain, "Results");
        }

        protected override IRunner CreateRunnerInAppDomain(ref AppDomain ad)
        {
            return CreateRunnerInAppDomain();
        }
    }

}
