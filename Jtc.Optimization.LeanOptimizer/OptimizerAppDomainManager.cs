using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Jtc.Optimization.LeanOptimizer.AppDomainExtensionMethods;

namespace Jtc.Optimization.LeanOptimizer
{

    public class OptimizerAppDomainManager
    {

        static AppDomainSetup _ads;
        static Dictionary<string, Dictionary<string, decimal>> _results;
        static object _resultsLocker;

        public static void Initialize()
        {
            _results = new Dictionary<string, Dictionary<string, decimal>>();
            _ads = SetupAppDomain();
            _resultsLocker = new object();
        }

        protected static AppDomainSetup SetupAppDomain()
        {
            // Construct and initialize settings for a second AppDomain.
            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            ads.DisallowBindingRedirects = false;
            ads.DisallowCodeDownload = true;
            //todo: core port
            //ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            return ads;
        }

        static Runner CreateRunnerInAppDomain(ref AppDomain ad)
        {
            // Create the second AppDomain.
            var name = Guid.NewGuid().ToString("x");
            //ad = AppDomain.CreateDomain(name, null, _ads);


            // Create an instance of MarshalbyRefType in the second AppDomain. 
            // A proxy to the object is returned.
            Runner rc = (Runner)ad.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(Runner).FullName);

            SetResults(ad, _results);

            return rc;
        }

        static Queuer CreateQueuerInAppDomain(ref AppDomain ad)
        {
            // Create the second AppDomain.
            var name = Guid.NewGuid().ToString("x");
            //ad = AppDomain.CreateDomain(name, null, _ads);

            // Create an instance of MarshalbyRefType in the second AppDomain. 
            // A proxy to the object is returned.
            Queuer rc = (Queuer)ad.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(Queuer).FullName);

            SetResults(ad, _results);

            return rc;
        }

        public static Dictionary<string, decimal> RunAlgorithm(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            AppDomain ad = null;
            Runner rc = CreateRunnerInAppDomain(ref ad);

            var result = rc.Run(list, config);

            lock (_resultsLocker)
            {
                foreach (var item in GetResults(ad))
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

        /// <summary>
        /// Can be used to "russian doll" QCAlgorithm
        /// </summary>
        /// <param name="list"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Tuple<AppDomain, Task<Dictionary<string, decimal>>> RunAlgorithmAsync(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            var runner = CreateRunnerInAppDomain(ref EngineContext.AppDomain);

            var result = Task.Run(() => runner.Run(list, config));

            return Tuple.Create(EngineContext.AppDomain, result);
        }

        public static Dictionary<string, Dictionary<string, decimal>> GetResults()
        {
            return _results;
        }

        public static Dictionary<string, Dictionary<string, decimal>> GetResults(AppDomain ad)
        {
            return GetData<Dictionary<string, Dictionary<string, decimal>>>(ad, "Results");
        }

        public static T GetData<T>(AppDomain ad, string key)
        {
            return (T)ad.GetData(key);
        }

        public static void SetResults(AppDomain ad, object item)
        {
            SetData(ad, "Results", item);
        }

        public static void SetData(AppDomain ad, string key, object item)
        {
            ad.SetData(key, item);
        }

    }

    public class AppDomainSetup
    {
        public string ApplicationBase { get; internal set; }
        public bool DisallowBindingRedirects { get; set; }
        public bool DisallowCodeDownload { get; internal set; }
        public object ConfigurationFile { get; internal set; }
    }

    public static class AppDomainExtensionMethods
    {
        public static object CreateInstanceAndUnwrap(this AppDomain domain, string a, string b)
        {
            throw new NotImplementedException();
        }
    }
}
