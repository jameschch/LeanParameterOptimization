using Jtc.Optimization.LeanOptimizer.Base;
using Jtc.Optimization.LeanOptimizer.Handlers;
using Jtc.Optimization.Objects.Interfaces;
using Newtonsoft.Json;
using QuantConnect;
using QuantConnect.Algorithm;
using QuantConnect.Configuration;
using QuantConnect.Data.Auxiliary;
using QuantConnect.Interfaces;
using QuantConnect.Lean.Engine;
using QuantConnect.Lean.Engine.DataFeeds;
using QuantConnect.Lean.Engine.HistoricalData;
using QuantConnect.Lean.Engine.RealTime;
using QuantConnect.Lean.Engine.Server;
using QuantConnect.Lean.Engine.Setup;
using QuantConnect.Lean.Engine.Storage;
using QuantConnect.Lean.Engine.TransactionHandlers;
using QuantConnect.Logging;
using QuantConnect.Packets;
using QuantConnect.Queues;
using QuantConnect.Statistics;
using QuantConnect.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Jtc.Optimization.LeanOptimizer
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
            return ResultMediator.GetData<Dictionary<string, Dictionary<string, decimal>>>(AppDomain.CurrentDomain, "Results");
        }

        public Dictionary<string, decimal> Run(Dictionary<string, object> items, IOptimizerConfiguration config)
        {

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

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
            ResultMediator.SetData(AppDomain.CurrentDomain, id, filteredConfig);

            LogProvider.TraceLogger.Trace($"id: {id} started.");
            LaunchLean(id);
            LogProvider.TraceLogger.Trace($"id: {id} finished.");


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

        private void LaunchLean(string id)
        {
            ConfigMerger.Merge(_config, id, this.GetType());            

            //todo: instance logging
            //var logFileName = "log" + DateTime.Now.ToString("yyyyMMddssfffffff") + "_" + id + ".txt";
            Log.LogHandler = LogSingleton.Instance;

            var jobQueue = new JobQueue();
            var manager = new LocalLeanManager();

            var systemHandlers = new LeanEngineSystemHandlers(
                jobQueue,
                new EmptyApiHandler(),
                new QuantConnect.Messaging.Messaging(),
                manager);

            systemHandlers.Initialize();

            var map = new LocalDiskMapFileProvider();
            var results = new OptimizerResultHandler();
            var transactions = new BacktestingTransactionHandler();
            var dataFeed = new FileSystemDataFeed();
            var realTime = new BacktestingRealTimeHandler();
            var data = new DefaultDataProvider();

            var leanEngineAlgorithmHandlers = new LeanEngineAlgorithmHandlers(
                    results,
                    new ConsoleSetupHandler(),
                    dataFeed,
                    transactions,
                    realTime,
                    map,
                    new LocalDiskFactorFileProvider(map),
                    data,
                    new OptimizerAlphaHandler(),
                    new EmptyObjectStore(),
                    new DataPermissionManager());
            _resultsHandler = (OptimizerResultHandler)leanEngineAlgorithmHandlers.Results;

            var job = (BacktestNodePacket)systemHandlers.JobQueue.NextJob(out var algorithmPath);
            //mark job with id. Is set on algorithm in OptimizerAlphaHandler
            job.BacktestId = id;
            //todo: pass period through job
            //job.PeriodStart = _config.StartDate;
            //job.PeriodFinish = _config.EndDate;

            Engine engine;
            AlgorithmManager algorithmManager;
            try
            {
                algorithmManager = new AlgorithmManager(false);
                systemHandlers.LeanManager.Initialize(systemHandlers, leanEngineAlgorithmHandlers, job, algorithmManager);
                engine = new Engine(systemHandlers, leanEngineAlgorithmHandlers, false);
                using (var workerThread = new MultipleWorkerThread())
                {
                    engine.Run(job, algorithmManager, algorithmPath, workerThread);
                }
            }
            finally
            {
                // clean up resources
                Composer.Instance.Reset();
                results.Charts.Clear();
                results.Messages.Clear();
                if (results.Algorithm != null)
                {
                    results.Algorithm.Transactions.TransactionRecord.Clear();
                    results.Algorithm.SubscriptionManager.Subscriptions.SelectMany(s => s.Consolidators)?.ToList().ForEach(f =>
                    {
                        results.Algorithm.SubscriptionManager.RemoveConsolidator(f.WorkingData?.Symbol, f);
                        UnregisterAllEvents(f);
                    });
                    if (results.Algorithm is QCAlgorithm)
                    {
                        ((QCAlgorithm)results.Algorithm).SubscriptionManager.Subscriptions.ToList().Clear();
                    }
                    if (_config.AlgorithmLanguage != "Python")
                    {
                        results.Algorithm.HistoryProvider = null;
                    }
                    var closedTrades = (List<Trade>)typeof(TradeBuilder).GetField("_closedTrades", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(results.Algorithm.TradeBuilder);
                    closedTrades.Clear();
                    results.Algorithm = null;
                }
                transactions.Orders.Clear();
                transactions.OrderTickets.Clear();
                manager.Dispose();
                systemHandlers.Dispose();
                leanEngineAlgorithmHandlers.Dispose();
                results = null;
                dataFeed = null;
                transactions = null;
                realTime = null;
                data = null;
                map = null;
                systemHandlers = null;
                leanEngineAlgorithmHandlers = null;
                algorithmManager = null;
                engine = null;
                job = null;
                jobQueue = null;
                manager = null;
            }

        }

        //fix base consolidator event handler leak
        private static void UnregisterAllEvents(object objectWithEvents)
        {
            Type theType = objectWithEvents.GetType().BaseType;
            foreach (FieldInfo field in theType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                EventInfo eventInfo = theType.GetEvent(field.Name);
                if (eventInfo != null)
                {
                    MulticastDelegate multicastDelegate = field.GetValue(objectWithEvents) as MulticastDelegate;
                    if (multicastDelegate != null)
                    {
                        foreach (Delegate _delegate in multicastDelegate.GetInvocationList())
                        {
                            eventInfo.RemoveEventHandler(objectWithEvents, _delegate);
                        }
                    }
                }
            }
        }

        //due to single app domain, multiple instances of worker thread are needed.
        private class MultipleWorkerThread : WorkerThread
        {
            public MultipleWorkerThread() : base()
            {
            }
        }

    }
}
