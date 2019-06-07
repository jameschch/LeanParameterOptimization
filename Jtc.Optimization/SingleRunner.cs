using Newtonsoft.Json;
using QuantConnect.Configuration;
using QuantConnect.Data.Auxiliary;
using QuantConnect.Lean.Engine;
using QuantConnect.Lean.Engine.DataFeeds;
using QuantConnect.Lean.Engine.RealTime;
using QuantConnect.Lean.Engine.Server;
using QuantConnect.Lean.Engine.Setup;
using QuantConnect.Lean.Engine.TransactionHandlers;
using QuantConnect.Logging;
using QuantConnect.Packets;
using QuantConnect.Queues;
using QuantConnect.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Optimization
{

    /// <summary>
    /// Run multiple iterations in a single App domain to allow access to shared object instances across algorithm executions
    /// </summary>
    public class SingleRunner : MarshalByRefObject, IRunner
    {

        private OptimizerResultHandler _resultsHandler;
        IOptimizerConfiguration _config;
        private object _resultsLocker = new object();
        private Dictionary<string, Dictionary<string, decimal>> GetResults()
        {
            return SingleAppDomainManager.GetData<Dictionary<string, Dictionary<string, decimal>>>(AppDomain.CurrentDomain, "Results");
        }

        public Dictionary<string, decimal> Run(Dictionary<string, object> items, IOptimizerConfiguration config)
        {
            _config = config;


            var id = (items.ContainsKey("Id") ? items["Id"] : Guid.NewGuid().ToString("N")).ToString();

            if (_config.StartDate.HasValue && _config.EndDate.HasValue)
            {
                if (!items.ContainsKey("startDate")) { items.Add("startDate", _config.StartDate); }
                if (!items.ContainsKey("endDate")) { items.Add("endDate", _config.EndDate); }
            }

            string jsonKey = JsonConvert.SerializeObject(items.Where(i => i.Key != "Id"));

            if (!_config.EnableRunningDuplicateParameters && GetResults().ContainsKey(jsonKey))
            {
                return GetResults()[jsonKey];
            }

            var filteredConfig = new Dictionary<string, object>();
            //just ignore id gene
            foreach (var pair in items.Where(i => i.Key != "Id"))
            {
                if (pair.Value is DateTime?)
                {
                    var cast = ((DateTime?)pair.Value);
                    if (cast.HasValue)
                    {
                        filteredConfig.Add(pair.Key, cast.Value.ToString("O"));
                    }
                }
                else
                {
                    filteredConfig.Add(pair.Key, pair.Value.ToString());
                }
            }

            //store uniquely keyed config in app domain for each algorithm instance
            //todo: cleanup
            SingleAppDomainManager.SetData(AppDomain.CurrentDomain, id, filteredConfig);

            LaunchLean(id);

            AddToResults(config, jsonKey);

            return _resultsHandler.FullResults;
        }

        private void AddToResults(IOptimizerConfiguration config, string jsonKey)
        {
            lock (_resultsLocker)
            {
                //for multiple runs, keep most recent only
                if (config.EnableRunningDuplicateParameters)
                {
                    if (GetResults().ContainsKey(jsonKey))
                    {
                        GetResults().Remove(jsonKey);
                    }

                    GetResults().Add(jsonKey, _resultsHandler.FullResults);
                }
                else
                {
                    if (!GetResults().ContainsKey(jsonKey))
                    {
                        GetResults().Add(jsonKey, _resultsHandler.FullResults);
                    }
                }
            }
        }

        private void AddToResults()
        { }

        private void LaunchLean(string id)
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
                filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(filename) + id + Path.GetExtension(filename));

                Config.Set("transaction-log", filename);
            }

            Config.Set("api-handler", nameof(EmptyApiHandler));
            Config.Set("alpha-handler", nameof(OptimizerAlphaHandler));
            Config.Set("backtesting.result-handler", nameof(OptimizerResultHandler));

            Composer.Instance.Reset();

            //todo: instance logging
            //var logFileName = "log" + DateTime.Now.ToString("yyyyMMddssfffffff") + "_" + id + ".txt";
            Log.LogHandler = LogSingleton.Instance;

            var systemHandlers = new LeanEngineSystemHandlers(
                new JobQueue(),
                new EmptyApiHandler(),
                new QuantConnect.Messaging.Messaging(),
                new LocalLeanManager());

            systemHandlers.Initialize();

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
                    new OptimizerAlphaHandler());
            _resultsHandler = (OptimizerResultHandler)leanEngineAlgorithmHandlers.Results;

            var job = (BacktestNodePacket)systemHandlers.JobQueue.NextJob(out var algorithmPath);
            //mark job with id. Is set on algorithm in OptimizerAlphaHandler
            job.BacktestId = id;

            try
            {
                var algorithmManager = new AlgorithmManager(false);
                systemHandlers.LeanManager.Initialize(systemHandlers, leanEngineAlgorithmHandlers, job, algorithmManager);
                var engine = new Engine(systemHandlers, leanEngineAlgorithmHandlers, false);
                engine.Run(job, algorithmManager, algorithmPath);
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
