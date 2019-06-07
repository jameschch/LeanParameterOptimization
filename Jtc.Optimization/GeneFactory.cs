using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization
{

    public class GeneFactory
    {

        private static Random _random = new Random();
        public static GeneConfiguration[] Config { get; private set; }
        private static IRandomization _basic;
        private static IRandomization _fibonacci;

        public static void Initialize(GeneConfiguration[] config)
        {
            Config = config;
            _basic = RandomizationProvider.Current;
            _fibonacci = new FibonacciRandomization();
        }

        public static int RandomBetween(int minValue, int maxValue)
        {
            return RandomizationProvider.Current.GetInt(minValue, maxValue + 1);
        }

        public static decimal RandomBetween(decimal minValue, decimal maxValue, int? precision = null)
        {
            if (!precision.HasValue)
            {
                precision = GetPrecision(minValue);
            }

            var value = RandomizationProvider.Current.GetDouble() * ((double)maxValue - (double)minValue) + (double)minValue;
            return (decimal)System.Math.Round(value, precision.Value);
        }

        public static Gene Generate(GeneConfiguration config, bool isActual)
        {
            if (config.Fibonacci && RandomizationProvider.Current.GetType() != typeof(FibonacciRandomization))
            {
                RandomizationProvider.Current = _fibonacci;
            }
            else
            {
                RandomizationProvider.Current = _basic;
            }

            if (isActual && config.ActualInt.HasValue)
            {
                return new Gene(new KeyValuePair<string, object>(config.Key, config.ActualInt));
            }
            else if (isActual && config.ActualDecimal.HasValue)
            {
                return new Gene(new KeyValuePair<string, object>(config.Key, config.ActualDecimal));
            }
            else if (config.MinDecimal.HasValue && config.MaxDecimal.HasValue)
            {
                return new Gene(new KeyValuePair<string, object>(config.Key, GeneFactory.RandomBetween(config.MinDecimal.Value, config.MaxDecimal.Value, config.Precision)));
            }

            return new Gene(new KeyValuePair<string, object>(config.Key, GeneFactory.RandomBetween(config.MinInt.Value, config.MaxInt.Value)));
        }

        public static int? GetPrecision(decimal value)
        {
            return BitConverter.GetBytes(decimal.GetBits(value)[3])[2];
        }

    }
}
