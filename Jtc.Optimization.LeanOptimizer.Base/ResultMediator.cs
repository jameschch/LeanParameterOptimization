using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.LeanOptimizer.Base
{
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
