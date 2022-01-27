using QuantConnect.Data.Auxiliary;
using QuantConnect.Lean.Engine;
using QuantConnect.Lean.Engine.DataFeeds;
using QuantConnect.Lean.Engine.RealTime;
using QuantConnect.Lean.Engine.Server;
using QuantConnect.Lean.Engine.Setup;
using QuantConnect.Lean.Engine.TransactionHandlers;
using QuantConnect.Logging;
using QuantConnect.Messaging;
using QuantConnect.Packets;
using QuantConnect.Queues;
using QuantConnect.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jtc.Optimization.LeanOptimizer
{
    public class Runner : BaseRunner
    {

        private bool _disposed;

        protected override void LaunchLean(Dictionary<string, object> items, string id)
        {

            var systemHandlers = new LeanEngineSystemHandlers(
                new JobQueue(),
                new EmptyApiHandler(),
                new Messaging(),
                new LocalLeanManager());

            systemHandlers.Initialize();

            //separate log uniquely named
            var logFileName = "log" + DateTime.Now.ToString("yyyyMMddssfffffff") + "_" + id + ".txt";

            using (Log.LogHandler = new FileLogHandler(logFileName, true))
            {
                var leanEngineAlgorithmHandlers = new LeanEngineAlgorithmHandlers(
                        new OptimizerResultHandler(),
                        new ConsoleSetupHandler(),
                        new FileSystemDataFeed(),
                        new BacktestingTransactionHandler(),
                        new BacktestingRealTimeHandler(),
                        new LocalDiskMapFileProvider(),
                        new LocalDiskFactorFileProvider(),
                        new DefaultDataProvider(),
                        new OptimizerAlphaHandler(),
                        new EmptyObjectStore(),
                        new DataPermissionManager());

                _resultsHandler = (OptimizerResultHandler)leanEngineAlgorithmHandlers.Results;

                var job = (BacktestNodePacket)systemHandlers.JobQueue.NextJob(out var algorithmPath);
                job.BacktestId = id;
                //todo: pass period through job
                //job.PeriodStart = items.StartDate;
                //job.PeriodFinish = items.EndDate;

                AddJobParameters(items, job);

                try
                {
                    leanEngineAlgorithmHandlers.FactorFileProvider
                        .Initialize(leanEngineAlgorithmHandlers.MapFileProvider, leanEngineAlgorithmHandlers.DataProvider);
                    leanEngineAlgorithmHandlers.MapFileProvider
                        .Initialize(leanEngineAlgorithmHandlers.DataProvider);
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Runner()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                //todo:
            }

            _disposed = true;
        }


    }
}
