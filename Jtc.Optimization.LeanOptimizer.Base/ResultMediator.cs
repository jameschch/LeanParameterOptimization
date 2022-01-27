using System;
using System.Collections.Generic;

namespace Jtc.Optimization.LeanOptimizer.Base
{

    //todo: No longer any reason to pass through app domain? Could just be a singleton.
    public class ResultMediator
    {

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
}
