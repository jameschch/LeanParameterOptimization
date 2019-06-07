using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;

namespace Optimization
{

    public class SingleAppDomainManager : OptimizerAppDomainManager
    {

        static object _resultsLocker;

        public new static void Initialize()
        {
            
            _resultsLocker = new object();

            SetResults(AppDomain.CurrentDomain, new Dictionary<string, Dictionary<string, decimal>>());
        }

        static SingleRunner CreateRunnerInAppDomain()
        {
            var rc = new SingleRunner();

            return rc;
        }

        public new static Dictionary<string, decimal> RunAlgorithm(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            var rc = CreateRunnerInAppDomain();

            var result = rc.Run(list, config);

            return result;
        }

    }

}
