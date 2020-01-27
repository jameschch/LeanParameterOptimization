using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantConnect.Interfaces;

namespace Jtc.Optimization.LeanOptimizer.Example
{

    /// <summary>
    /// Used in a shared AppDomain to provide parameter sets for each algorithm execution or falling back on shared config
    /// </summary>
    public class InstancedConfig
    {
        private readonly IAlgorithm _algorithm;

        public InstancedConfig(IAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            var type = typeof(T);

            var domainConfig = AppDomain.CurrentDomain.GetData(_algorithm.AlgorithmId);
            if (domainConfig != null && ((Dictionary<string, object>)domainConfig).ContainsKey(key))
            {

                var value = ((Dictionary<string, object>)domainConfig)[key];

                if (value == null)
                {
                    return defaultValue;
                }

                if (type.IsEnum)
                {
                    return (T)Enum.Parse(type, value.ToString());
                }

                if (typeof(IConvertible).IsAssignableFrom(type))
                {
                    return (T)Convert.ChangeType(value, type);
                }

                var parse = type.GetMethod("Parse", new[] { typeof(string) });
                if (parse != null)
                {
                    var result = parse.Invoke(null, new object[] { value });
                    return (T)result;
                }
            }

            var fromConfig = QuantConnect.Configuration.Config.GetValue(key, defaultValue);


            if (fromConfig != null && (fromConfig.Equals(defaultValue) || fromConfig.Equals(default(T))))
            {
                var fromParam = _algorithm.GetParameter(key);
                if (type.IsEnum)
                {
                    return (T)Enum.Parse(type, fromParam);
                }

                if (!string.IsNullOrEmpty(fromParam?.Trim()) && typeof(IConvertible).IsAssignableFrom(type))
                {
                    return (T)Convert.ChangeType(fromParam, typeof(T));
                }
            }

            return fromConfig;

        }

    }
}
