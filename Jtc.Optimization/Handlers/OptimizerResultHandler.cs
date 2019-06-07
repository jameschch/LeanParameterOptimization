using QuantConnect;
using QuantConnect.Lean.Engine.Results;
using QuantConnect.Orders;
using QuantConnect.Packets;
using QuantConnect.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantConnect.Interfaces;
using QuantConnect.Lean.Engine.DataFeeds;
using QuantConnect.Lean.Engine.Setup;
using QuantConnect.Lean.Engine.TransactionHandlers;
using System.Collections.Concurrent;
using QuantConnect.Securities;

namespace Optimization
{
    public class OptimizerResultHandler : IResultHandler
    {

        private IResultHandler _shadow;

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
        #endregion

        public OptimizerResultHandler()
        {
            _shadow = new BacktestingResultHandler();

        }

        public void SendFinalResult(AlgorithmNodePacket job, Dictionary<int, Order> orders, Dictionary<DateTime, decimal> profitLoss, Dictionary<string, Holding> holdings,
            CashBook cashbook, StatisticsResults statisticsResults, Dictionary<string, string> banner)
        {
            _shadow.SendFinalResult(job, orders, profitLoss, holdings, cashbook, statisticsResults, banner);

            if (_hasError)
            {
                FullResults = null;
                return;
            }

            FullResults = StatisticsAdapter.Transform(statisticsResults.TotalPerformance, statisticsResults.Summary);
        }

        #region Shadow Methods
        public void Initialize(AlgorithmNodePacket job, IMessagingHandler messagingHandler, IApi api, IDataFeed dataFeed, ISetupHandler setupHandler, ITransactionHandler transactionHandler)
        {
            _shadow.Initialize(job, messagingHandler, api, setupHandler, transactionHandler);
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
            _hasError = true;
        }

        public void Sample(string chartName, string seriesName, int seriesIndex, SeriesType seriesType, DateTime time, decimal value, string unit = "$")
        {
            _shadow.Sample(chartName, seriesName, seriesIndex, seriesType, time, value, unit);
        }

        public void SampleEquity(DateTime time, decimal value)
        {
            _shadow.SampleEquity(time, value);
        }

        public void SamplePerformance(DateTime time, decimal value)
        {
            _shadow.SamplePerformance(time, value);
        }

        public void SampleBenchmark(DateTime time, decimal value)
        {
            _shadow.SampleBenchmark(time, value);
        }

        public void SampleAssetPrices(Symbol symbol, DateTime time, decimal value)
        {
            _shadow.SampleAssetPrices(symbol, time, value);
        }

        public void SampleRange(List<Chart> samples)
        {
            _shadow.SampleRange(samples);
        }

        public void SetAlgorithm(IAlgorithm algorithm)
        {
            _shadow.SetAlgorithm(algorithm);
        }

        public void StoreResult(Packet packet, bool async = false)
        {
            _shadow.StoreResult(packet, async);
        }

        public void SendStatusUpdate(AlgorithmStatus status, string message = "")
        {
            _shadow.SendStatusUpdate(status, message);
        }

        public void SetChartSubscription(string symbol)
        {
            _shadow.SetChartSubscription(symbol);
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
            _shadow.Exit();
        }

        public void PurgeQueue()
        {
            _shadow.PurgeQueue();
        }

        public void ProcessSynchronousEvents(bool forceProcess = false)
        {
            _shadow.ProcessSynchronousEvents(forceProcess);
        }

        public string SaveLogs(string id, IEnumerable<string> logs)
        {
            return _shadow.SaveLogs(id, logs);
        }

        public void SaveResults(string name, Result result)
        {
            _shadow.SaveResults(name, result);
        }

        public void SetAlphaRuntimeStatistics(AlphaRuntimeStatistics statistics)
        {
            _shadow.SetAlphaRuntimeStatistics(statistics);
        }

        public void Initialize(AlgorithmNodePacket job, IMessagingHandler messagingHandler, IApi api, ISetupHandler setupHandler, ITransactionHandler transactionHandler)
        {
            _shadow.Initialize(job, messagingHandler, api, setupHandler, transactionHandler);
        }

        public void SetDataManager(IDataFeedSubscriptionManager dataManager)
        {
            _shadow.SetDataManager(dataManager);
        }
        #endregion
    }
}
