using Moq;
using NUnit.Framework;
using QuantConnect;
using QuantConnect.Interfaces;
using QuantConnect.Lean.Engine.Results;
using QuantConnect.Lean.Engine.TransactionHandlers;
using QuantConnect.Notifications;
using QuantConnect.Orders;
using QuantConnect.Packets;
using QuantConnect.Securities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer.Tests.Handlers
{
    [TestFixture]
    public class OptimizerResultHandlerTests
    {
        [Test]
        public void OptimizerResultHandlerTest()
        {

            var shadow = new Wrapper();

            Type shadowType = shadow.GetType();

            var unit = new OptimizerResultHandler(shadow);
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            var algorithmMock = new Mock<IAlgorithm>();
            var securityManager = new SecurityManager(Mock.Of<ITimeKeeper>());
            var securityTransactionManager = new SecurityTransactionManager(algorithmMock.Object, securityManager);
            var orderProcessor = new Mock<IOrderProcessor>();
            orderProcessor.Setup(o => o.GetOpenOrders(It.IsAny<Func<Order, bool>>())).Returns(new List<Order>());
            securityTransactionManager.SetOrderProcessor(orderProcessor.Object);

            algorithmMock.Setup(a => a.Portfolio).Returns(new SecurityPortfolioManager(securityManager,securityTransactionManager));

            var tradeBuilder = new Mock<ITradeBuilder>();
            tradeBuilder.Setup(t => t.ClosedTrades).Returns(new List<QuantConnect.Statistics.Trade>());
            algorithmMock.Setup(a => a.TradeBuilder).Returns(tradeBuilder.Object);

            unit.SetAlgorithm(algorithmMock.Object, 100);

            shadowType.GetProperty("Algorithm", flags).SetValue(shadow, algorithmMock.Object);

            securityTransactionManager.TransactionRecord.Add(DateTime.Now, 100);
            algorithmMock.Setup(s => s.Transactions).Returns(securityTransactionManager);

            var transactionHandler = new Mock<ITransactionHandler>();
            transactionHandler.Setup(v => v.Orders).Returns(new ConcurrentDictionary<int, Order>());
            shadowType.GetField("TransactionHandler", flags).SetValue(shadow, transactionHandler.Object);

            var startTime = shadowType.BaseType.BaseType.GetRuntimeFields().Single(s => s.Name.Contains("StartTime"));
            startTime.SetValue(shadow, DateTime.Now);

            var messagingHandler = new MessagingWrapper();
            shadowType.GetField("MessagingHandler", flags).SetValue(shadow, messagingHandler);

            unit.SendFinalResult();

            Assert.True((bool)shadowType.BaseType.GetField("_processingFinalPacket", flags).GetValue(shadow));
            transactionHandler.Verify(v => v.Orders);
            Assert.True(messagingHandler.SendWasCalled);

            Assert.AreEqual(20, unit.FullResults.Count());
        }
    }

    public class Wrapper : BacktestingResultHandler
    {

        public Wrapper() : base()
        {
            Charts.AddOrUpdate("Benchmark", new Chart("Benchmark"));
            Charts["Benchmark"].Series.Add("Benchmark", new Series("Benchmark", SeriesType.Candle, 0, "$"));
            Charts["Strategy Equity"].Series["Equity"].AddPoint(new ChartPoint(DateTime.Now, 1));
        }

        public override void SetAlgorithm(IAlgorithm algorithm, decimal startingPortfolioValue)
        {
        }

        protected new Dictionary<string, string> GetAlgorithmRuntimeStatistics(Dictionary<string, string> runtimeStatistics = null, bool addColon = false)
        {
            return runtimeStatistics;
        }

    }

    public class MessagingWrapper : IMessagingHandler
    {
        public bool HasSubscribers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool SendWasCalled { get; set; }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Send(Packet packet)
        {
            SendWasCalled = true;
        }

        public void SendNotification(Notification notification)
        {
            throw new NotImplementedException();
        }

        public void SetAuthentication(AlgorithmNodePacket job)
        {
            throw new NotImplementedException();
        }
    }
}
