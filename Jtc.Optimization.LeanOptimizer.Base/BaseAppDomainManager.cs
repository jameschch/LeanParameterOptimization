using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer.Base
{

    public abstract class BaseAppDomainManager<T> where T : BaseAppDomainManager<T>, new()
    {

        Dictionary<string, Dictionary<string, decimal>> _results;
        static object _resultsLocker;

        private static T _instance = new T();
        protected Type RunnerType;

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        static BaseAppDomainManager()
        {
        }

        protected BaseAppDomainManager()
        {
            if (_instance != null)
            {
                throw new Exception("Unsupported call to singleton constructor");
            }
            _resultsLocker = new object();
            _results = new Dictionary<string, Dictionary<string, decimal>>();
        }

        protected abstract IRunner CreateRunnerInAppDomain(ref AppDomain ad);

        public virtual Dictionary<string, decimal> RunAlgorithm(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            AppDomain ad = null;
            var runner = CreateRunnerInAppDomain(ref ad);

            ResultMediator.SetResults(ad, _results);

            var result = runner.Run(list, config);

            lock (_resultsLocker)
            {
                foreach (var item in ResultMediator.GetResults(ad))
                {
                    if (!_results.ContainsKey(item.Key))
                    {
                        _results.Add(item.Key, item.Value);
                    }
                }
            }

            AppDomain.Unload(ad);

            return result;
        }

        public Dictionary<string, Dictionary<string, decimal>> GetResults()
        {
            return _results;
        }

    }

}
