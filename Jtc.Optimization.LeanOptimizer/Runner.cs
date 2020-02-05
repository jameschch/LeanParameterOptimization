using Jtc.Optimization.LeanOptimizer.Base;
using Jtc.Optimization.LeanOptimizer.Legacy;
using Jtc.Optimization.Objects.Interfaces;
using Newtonsoft.Json;
using QuantConnect.Configuration;
using QuantConnect.Data.Auxiliary;
using QuantConnect.Lean.Engine;
using QuantConnect.Lean.Engine.DataFeeds;
using QuantConnect.Lean.Engine.RealTime;
using QuantConnect.Lean.Engine.Server;
using QuantConnect.Lean.Engine.Setup;
using QuantConnect.Lean.Engine.Storage;
using QuantConnect.Lean.Engine.TransactionHandlers;
using QuantConnect.Logging;
using QuantConnect.Messaging;
using QuantConnect.Packets;
using QuantConnect.Queues;
using QuantConnect.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Jtc.Optimization.LeanOptimizer
{
    public class Runner : MarshalByRefObject, IRunner
    {

        private OptimizerResultHandler _resultsHandler;
        IOptimizerConfiguration _config;
        private string _id;

        public Dictionary<string, decimal> Run(Dictionary<string, object> items, IOptimizerConfiguration config)
        {

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Dictionary<string, Dictionary<string, decimal>> results = ResultMediator.GetResults(AppDomain.CurrentDomain);
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

            LogProvider.TraceLogger.Trace($"id: {_id} started.");
            LaunchLean();
            LogProvider.TraceLogger.Trace($"id: {_id} finished.");

            if (_resultsHandler.FullResults != null && _resultsHandler.FullResults.Any())
            {
                if (config.EnableRunningDuplicateParameters && results.ContainsKey(jsonKey))
                {
                    results.Remove(jsonKey);
                }
                results.Add(jsonKey, _resultsHandler.FullResults);
                ResultMediator.SetResults(AppDomain.CurrentDomain, results);
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

            var systemHandlers = new LeanEngineSystemHandlers(
                new JobQueue(),
                new EmptyApiHandler(),
                new Messaging(),
                new LocalLeanManager());

            systemHandlers.Initialize();

            //separate log uniquely named
            var logFileName = "log" + DateTime.Now.ToString("yyyyMMddssfffffff") + "_" + _id + ".txt";

            using (Log.LogHandler = new FileLogHandler(logFileName, true))
            {
                //override config to use custom result handler
                Config.Set("backtesting.result-handler", nameof(OptimizerResultHandler));

                var map = new LocalDiskMapFileProvider();
                var leanEngineAlgorithmHandlers = new LeanEngineAlgorithmHandlers(
                        new OptimizerResultHandler(),
                        new ConsoleSetupHandler(),
                        new FileSystemDataFeed(),
                        new BacktestingTransactionHandler(),
                        new BacktestingRealTimeHandler(),
                        map,
                        new LocalDiskFactorFileProvider(map),
                        new DefaultDataProvider(),
                        new OptimizerAlphaHandler(),
                        new EmptyObjectStore());

                _resultsHandler = (OptimizerResultHandler)leanEngineAlgorithmHandlers.Results;

                var job = (BacktestNodePacket)systemHandlers.JobQueue.NextJob(out var algorithmPath);
                //todo: pass period through job
                //job.PeriodStart = _config.StartDate;
                //job.PeriodFinish = _config.EndDate;

                try
                {
                    var algorithmManager = new AlgorithmManager(false);
                    systemHandlers.LeanManager.Initialize(systemHandlers, leanEngineAlgorithmHandlers, job, algorithmManager);
                    var engine = new Engine(systemHandlers, leanEngineAlgorithmHandlers, false);
                    engine.Run(job, algorithmManager, algorithmPath, WorkerThread.Instance);
                }
                finally
                {
                    // clean up resources
                    systemHandlers.Dispose();
                    leanEngineAlgorithmHandlers.Dispose();
                }
            }
        }

    }
}
