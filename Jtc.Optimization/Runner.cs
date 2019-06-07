using Newtonsoft.Json;
using QuantConnect.Configuration;
using QuantConnect.Lean.Engine;
using QuantConnect.Logging;
using QuantConnect.Packets;
using QuantConnect.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Optimization
{
    public class Runner : MarshalByRefObject, IRunner
    {

        private OptimizerResultHandler _resultsHandler;
        IOptimizerConfiguration _config;
        private string _id;

        public Dictionary<string, decimal> Run(Dictionary<string, object> items, IOptimizerConfiguration config)
        {
            Dictionary<string, Dictionary<string, decimal>> results = OptimizerAppDomainManager.GetResults(AppDomain.CurrentDomain);
            _config = config;

            _id = (items.ContainsKey("Id") ? items["Id"] : Guid.NewGuid().ToString("N")).ToString();

            if (_config.StartDate.HasValue && _config.EndDate.HasValue)
            {
                if (!items.ContainsKey("startDate")) { items.Add("startDate", _config.StartDate); }
                if (!items.ContainsKey("endDate")) { items.Add("endDate", _config.EndDate); }
            }

            string jsonKey = JsonConvert.SerializeObject(items.Where(i => i.Key != "Id"));

            if (!config.EnableRunningDuplicateParameters && results.ContainsKey(jsonKey))
            {
                return results[jsonKey];
            }

            //just ignore id gene
            foreach (var pair in items.Where(i => i.Key != "Id"))
            {
                if (pair.Value is DateTime?)
                {
                    var cast = ((DateTime?)pair.Value);
                    if (cast.HasValue)
                    {
                        Config.Set(pair.Key, cast.Value.ToString("O"));
                    }
                }
                else
                {
                    Config.Set(pair.Key, pair.Value.ToString());
                }
            }

            LaunchLean();

            if (_resultsHandler.FullResults != null && _resultsHandler.FullResults.Any())
            {
                if (config.EnableRunningDuplicateParameters && results.ContainsKey(jsonKey))
                {
                    results.Remove(jsonKey);
                }
                results.Add(jsonKey, _resultsHandler.FullResults);
                OptimizerAppDomainManager.SetResults(AppDomain.CurrentDomain, results);
            }

            return _resultsHandler.FullResults;
        }

        private void LaunchLean()
        {
            Config.Set("environment", "backtesting");

            if (!string.IsNullOrEmpty(_config.AlgorithmTypeName))
            {
                Config.Set("algorithm-type-name", _config.AlgorithmTypeName);
            }

            if (!string.IsNullOrEmpty(_config.AlgorithmLocation))
            {
                Config.Set("algorithm-location", Path.GetFileName(_config.AlgorithmLocation));
            }

            if (!string.IsNullOrEmpty(_config.DataFolder))
            {
                Config.Set("data-folder", _config.DataFolder);
            }

            if (!string.IsNullOrEmpty(_config.TransactionLog))
            {
                var filename = _config.TransactionLog;
                filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    Path.GetFileNameWithoutExtension(filename) + _id + Path.GetExtension(filename));

                Config.Set("transaction-log", filename);
            }

            //transaction-log

            Config.Set("api-handler", nameof(EmptyApiHandler));
            var systemHandlers = LeanEngineSystemHandlers.FromConfiguration(Composer.Instance);
            systemHandlers.Initialize();

            //separate log uniquely named
            var logFileName = "log" + DateTime.Now.ToString("yyyyMMddssfffffff") + "_" + _id + ".txt";

            using (Log.LogHandler = new FileLogHandler(logFileName, true))
            {
                LeanEngineAlgorithmHandlers leanEngineAlgorithmHandlers;
                try
                {
                    //override config to use custom result handler
                    Config.Set("backtesting.result-handler", nameof(OptimizerResultHandler));
                    leanEngineAlgorithmHandlers = LeanEngineAlgorithmHandlers.FromConfiguration(Composer.Instance);
                    _resultsHandler = (OptimizerResultHandler)leanEngineAlgorithmHandlers.Results;
                }
                catch (CompositionException compositionException)
                {
                    Log.Error("Engine.Main(): Failed to load library: " + compositionException);
                    throw;
                }
                string algorithmPath;
                AlgorithmNodePacket job = systemHandlers.JobQueue.NextJob(out algorithmPath);

                try
                {
                    var algorithmManager = new AlgorithmManager(false);
                    systemHandlers.LeanManager.Initialize(systemHandlers, leanEngineAlgorithmHandlers, job, algorithmManager);
                    var engine = new Engine(systemHandlers, leanEngineAlgorithmHandlers, false);
                    engine.Run(job, algorithmManager, algorithmPath);
                }
                finally
                {
                    Log.Trace("Engine.Main(): Packet removed from queue: " + job.AlgorithmId);

                    // clean up resources
                    systemHandlers.Dispose();
                    leanEngineAlgorithmHandlers.Dispose();
                }
            }
        }

    }
}
