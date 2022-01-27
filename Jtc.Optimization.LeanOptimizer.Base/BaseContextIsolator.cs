using Jtc.Optimization.Objects.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Jtc.Optimization.LeanOptimizer.Base
{

    public class BaseContextIsolator<T> where T : BaseContextIsolator<T>, new()
    {

        Dictionary<string, Dictionary<string, decimal>> _results;

        private static T _instance = new T();
        protected Type RunnerType;

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        static BaseContextIsolator()
        {
        }

        protected BaseContextIsolator()
        {
            if (_instance != null)
            {
                throw new Exception("Unsupported call to singleton constructor");
            }
            _results = new Dictionary<string, Dictionary<string, decimal>>();
        }

        public virtual Dictionary<string, decimal> RunAlgorithm(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            ResultMediator.SetResults(AppDomain.CurrentDomain, _results);

            var context = new MultipleAssemblyLoadContext(Guid.NewGuid().ToString("N"), config, true);

            using (context.EnterContextualReflection())
            {
                var assembly = context.LoadFromAssemblyName
                    (new AssemblyName("Jtc.Optimization.LeanOptimizer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));

                var created = assembly.CreateInstance("Jtc.Optimization.LeanOptimizer.Runner");

                context.LoadFromAssemblyPath(Path.GetFullPath(config.AlgorithmLocation));

                var runner = (IRunner)created;
                var result = runner.Run(list, config);

                return result;
            }
        }

        public Dictionary<string, Dictionary<string, decimal>> GetResults()
        {
            return ResultMediator.GetResults(AppDomain.CurrentDomain);
        }

    }

}
