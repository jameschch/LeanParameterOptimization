using Harmony;
using QuantConnect;
using QuantConnect.Interfaces;
using QuantConnect.Lean.Engine.DataFeeds;
using QuantConnect.Lean.Engine.Results;
using QuantConnect.Lean.Engine.Setup;
using QuantConnect.Lean.Engine.TransactionHandlers;
using QuantConnect.Logging;
using QuantConnect.Orders;
using QuantConnect.Packets;
using QuantConnect.Securities;
using QuantConnect.Statistics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer
{
    public class OptimizerResultHandler : IResultHandler
    {
        protected IAlgorithm Algorithm { get; set; }

        private const string PatchMethod = "SaveResults";
        private BacktestingResultHandler _shadow;

        #region Properties
        public Dictionary<string, decimal> FullResults { get; set; }

        public ConcurrentQueue<Packet> Messages
        {
            get
            {
                return _shadow.Messages;
            }

            set
            {
                _shadow.Messages = value;
            }
        }

        public ConcurrentDictionary<string, Chart> Charts
        {
            get
            {
                return _shadow.Charts;
            }
            set
            {
                _shadow.Charts = value;
            }
        }

        public TimeSpan ResamplePeriod => _shadow.ResamplePeriod;

        public TimeSpan NotificationPeriod => _shadow.NotificationPeriod;

        public bool IsActive => _shadow.IsActive;

        private bool _hasError;
        private static Type _shadowType = typeof(BacktestingResultHandler);
        private static BindingFlags _flags = BindingFlags.Instance | BindingFlags.NonPublic;
        static object _locker = new object();

        #endregion

        public OptimizerResultHandler()
        {
            _shadow = new BacktestingResultHandler();
            Patch();
        }

        public OptimizerResultHandler(BacktestingResultHandler handler)
        {
            _shadow = handler;
            Patch();
        }

        private void Patch()
        {
            lock (_locker)
            {
                var harmony = HarmonyInstance.Create(PatchMethod);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
        }

        [HarmonyPatch(typeof(BaseResultsHandler))]
        [HarmonyPatch(PatchMethod)]
        class SaveResultsPatch
        {
            static bool Prefix(string name, Result result)
            {
                return false;
            }
        }

        //HACK: calculate and retain full statistics but not store result
        public void SendFinalResult()
        {
            if (_hasError)
            {
                FullResults = null;
                return;
            }

            try
            {

                //_processingFinalPacket = true;
                _shadowType.GetField("_processingFinalPacket", _flags).SetValue(_shadow, true);


                var charts = new Dictionary<string, Chart>(_shadow.Charts);
                //var orders = new Dictionary<int, Order>(_shadow.TransactionHandler.Orders);
                var transactionHandler = (ITransactionHandler)_shadowType.GetField("TransactionHandler", _flags).GetValue(_shadow);
                var orders = new Dictionary<int, Order>(transactionHandler.Orders);

                var profitLoss = new SortedDictionary<DateTime, decimal>(Algorithm.Transactions.TransactionRecord);

                //var statisticsResults = GenerateStatisticsResults(charts, profitLoss);
                var statisticsResults = (StatisticsResults)_shadowType.BaseType.InvokeMember("GenerateStatisticsResults", _flags | BindingFlags.InvokeMethod, null, _shadow,
                   new object[] { charts, profitLoss });

                //var runtime = GetAlgorithmRuntimeStatistics(statisticsResults.Summary);      
                var runtime = (Dictionary<string, string>)typeof(BaseResultsHandler).InvokeMember("GetAlgorithmRuntimeStatistics", _flags | BindingFlags.InvokeMethod, Type.DefaultBinder, _shadow,
                    new object[] { statisticsResults.Summary, null });

                FullResults = StatisticsAdapter.Transform(statisticsResults.TotalPerformance, statisticsResults.Summary);

                //FinalStatistics = statisticsResults.Summary;
                _shadowType.GetProperty("FinalStatistics").SetValue(_shadow, statisticsResults.Summary, _flags, null, null, null);

                foreach (var ap in statisticsResults.RollingPerformances.Values)
                {
                    ap.ClosedTrades.Clear();
                }

                //var result = new BacktestResultPacket(_job,
                //    new BacktestResult(charts, orders, profitLoss, statisticsResults.Summary, runtime, statisticsResults.RollingPerformances, statisticsResults.TotalPerformance)
                //    { AlphaRuntimeStatistics = AlphaRuntimeStatistics }, Algorithm.EndDate, Algorithm.StartDate)
                //{
                //    ProcessingTime = (DateTime.UtcNow - StartTime).TotalSeconds,
                //    DateFinished = DateTime.Now,
                //    Progress = 1
                //};
                var job = (BacktestNodePacket)_shadowType.GetField("_job", _flags).GetValue(_shadow);
                var startTime = (DateTime)_shadowType.GetProperty("StartTime", _flags).GetValue(_shadow);
                var alphaRuntimeStatistics = (AlphaRuntimeStatistics)_shadowType.GetProperty("AlphaRuntimeStatistics", _flags).GetValue(_shadow);

                var result = new BacktestResultPacket(job,
                    new BacktestResult(charts, orders, profitLoss, statisticsResults.Summary, runtime, statisticsResults.RollingPerformances, statisticsResults.TotalPerformance)
                    { AlphaRuntimeStatistics = alphaRuntimeStatistics }, Algorithm.EndDate, Algorithm.StartDate)
                {
                    ProcessingTime = (DateTime.UtcNow - startTime).TotalSeconds,
                    DateFinished = DateTime.Now,
                    Progress = 1
                };

                //StoreResult(result);
                //do not store result

                //MessagingHandler.Send(result);
                var messagingHandler = (IMessagingHandler)_shadowType.GetField("MessagingHandler", _flags).GetValue(_shadow);
                messagingHandler.Send(result);

            }
            catch (Exception ex)
            {
                LogProvider.ErrorLogger.Error(ex);
            }

        }

        #region Shadow Methods
        public void Initialize(AlgorithmNodePacket job, IMessagingHandler messagingHandler, IApi api, IDataFeed dataFeed, ITransactionHandler transactionHandler)
        {
            _shadow.Initialize(job, messagingHandler, api, transactionHandler);
        }

        public void Run()
        {
            _hasError = false;
            _shadow.Run();
        }

        public void DebugMessage(string message)
        {
            _shadow.DebugMessage(message);
        }

        public void SystemDebugMessage(string message)
        {
            _shadow.SystemDebugMessage(message);
        }

        public void SecurityType(List<SecurityType> types)
        {
            _shadow.SecurityType(types);
        }

        public void LogMessage(string message)
        {
            _shadow.LogMessage(message);
        }

        public void ErrorMessage(string error, string stacktrace = "")
        {
            _shadow.ErrorMessage(error, stacktrace);
        }

        public void RuntimeError(string message, string stacktrace = "")
        {
            _shadow.ErrorMessage(message, stacktrace);
            LogProvider.ErrorLogger.Error(new Exception($"{Algorithm.AlgorithmId}:{ message }:{stacktrace}"));
            _hasError = true;
        }

        public void Sample(DateTime time, bool force = false)
        {
            _shadow.Sample(time, force);
        }

        public void SampleEquity(DateTime time, decimal value)
        {
            _shadowType.InvokeMember("SampleEquity", _flags | BindingFlags.InvokeMethod, Type.DefaultBinder, _shadow, new object[] { time, value });
        }

        public void SampleRange(List<Chart> samples)
        {
            _shadowType.InvokeMember("SampleRange", _flags | BindingFlags.InvokeMethod, Type.DefaultBinder, _shadow, new object[] { samples });
        }

        public void SetAlgorithm(IAlgorithm algorithm, decimal startingPortfolioValue)
        {
            Algorithm = algorithm;
            _shadow.SetAlgorithm(algorithm, startingPortfolioValue);

        }

        public void StoreResult(Packet packet, bool async = false)
        {
            //do not save rounded results to disk
            //_shadow.StoreResult(packet, async);
        }

        public void SendStatusUpdate(AlgorithmStatus status, string message = "")
        {
            _shadow.SendStatusUpdate(status, message);
        }

        public void RuntimeStatistic(string key, string value)
        {
            _shadow.RuntimeStatistic(key, value);
        }

        public void OrderEvent(OrderEvent newEvent)
        {
            _shadow.OrderEvent(newEvent);
        }

        public void Exit()
        {
            //don't save logs
            var field = _shadowType.GetField("ExitTriggered", _flags);
            var exitTriggered = (bool)field.GetValue(_shadow);
            if (!exitTriggered)
            {
                ProcessSynchronousEvents(true);
            }

            field.SetValue(_shadow, true);
        }

        public void PurgeQueue()
        {
            _shadow.PurgeQueue();
        }

        public void ProcessSynchronousEvents(bool forceProcess = false)
        {
            _shadow.ProcessSynchronousEvents(forceProcess);
        }

        public string SaveLogs(string id, List<LogEntry> logs)
        {
            //do not use log file due to parallel locking
            //return _shadow.SaveLogs(id, logs);
            return null;
        }

        public void SaveResults(string name, Result result)
        {
            //do not save rounded results to disk
            //_shadow.SaveResults(name, result);
        }

        public void SetAlphaRuntimeStatistics(AlphaRuntimeStatistics statistics)
        {
            _shadow.SetAlphaRuntimeStatistics(statistics);
        }

        public void Initialize(AlgorithmNodePacket job, IMessagingHandler messagingHandler, IApi api, ITransactionHandler transactionHandler)
        {
            _shadow.Initialize(job, messagingHandler, api, transactionHandler);
        }

        public void SetDataManager(IDataFeedSubscriptionManager dataManager)
        {
            _shadow.SetDataManager(dataManager);
        }
        #endregion
    }
}
