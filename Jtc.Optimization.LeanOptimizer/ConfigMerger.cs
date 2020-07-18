using Jtc.Optimization.LeanOptimizer.Handlers;
using Jtc.Optimization.Objects.Interfaces;
using QuantConnect.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jtc.Optimization.LeanOptimizer
{
    class ConfigMerger
    {
        public static void Merge(IOptimizerConfiguration config, string id, Type runnerType)
        {
            Config.Set("environment", "backtesting");

            if (!string.IsNullOrEmpty(config.AlgorithmTypeName))
            {
                Config.Set("algorithm-type-name", config.AlgorithmTypeName);
            }

            if (!string.IsNullOrEmpty(config.AlgorithmLocation))
            {
                Config.Set("algorithm-location", config.AlgorithmLocation);
            }

            if (!string.IsNullOrEmpty(config.AlgorithmLanguage))
            {
                Config.Set("algorithm-language", config.AlgorithmLanguage);
            }

            if (!string.IsNullOrEmpty(config.DataFolder))
            {
                Config.Set("data-folder", config.DataFolder);
            }

            //override config to use custom result handler
            Config.Set("backtesting.result-handler", nameof(OptimizerResultHandler));

            if (runnerType == typeof(SingleRunner))
            {
                Config.Set("backtesting.history-provider", nameof(OptimizerSubscriptionDataReaderHistoryProvider));
                Config.Set("api-handler", nameof(EmptyApiHandler));
            }

            //todo: transaction log
            if (!string.IsNullOrEmpty(config.TransactionLog))
            {
                var filename = config.TransactionLog;
                filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(filename) + id + Path.GetExtension(filename));

                Config.Set("transaction-log", filename);
            }


        }


    }
}
