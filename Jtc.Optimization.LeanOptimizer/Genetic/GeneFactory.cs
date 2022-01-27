﻿using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using Jtc.Optimization.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer
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

        public static double RandomBetween(double minValue, double maxValue, int? precision = null)
        {
            if (!precision.HasValue)
            {
                precision = GetPrecision(minValue);
            }

            var value = RandomizationProvider.Current.GetDouble() * ((double)maxValue - (double)minValue) + (double)minValue;
            return System.Math.Round(value, precision.Value);
        }

        public static Gene Generate(GeneConfiguration config, bool isActual)
        {
            if (config.Fibonacci && RandomizationProvider.Current.GetType() != typeof(FibonacciRandomization))
            {
                RandomizationProvider.Current = _fibonacci;
            }
            else if (_basic != null)
            {
                RandomizationProvider.Current = _basic;
            }

            if (isActual && config.Actual.HasValue)
            {
                return new Gene(new KeyValuePair<string, object>(config.Key, config.Actual));
            }

            return new Gene(new KeyValuePair<string, object>(config.Key, GeneFactory.RandomBetween(config.Min.Value, config.Max.Value, config.Precision)));
        }

        public static int? GetPrecision(double value)
        {
            return BitConverter.GetBytes(decimal.GetBits(Convert.ToDecimal(value))[3])[2];
        }

    }
}
