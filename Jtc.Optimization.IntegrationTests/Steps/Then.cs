using Jtc.Optimization.LeanOptimizer;
using Jtc.Optimization.LeanOptimizer.Legacy;
using Jtc.Optimization.Objects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Jtc.Optimization.IntegrationTests.Steps
{
    [Binding]
    public class Then
    {
        [Then(@"the Sharpe Ratio should be (.*)")]
        public void ThenTheSharpeRatioShouldBe(double p0)
        {
            var actual = GetResults();
            var predicted = GetPredicted(actual);
            Assert.AreEqual(p0, (double)predicted.Value["SharpeRatio"], 0.001);
        }

        private static KeyValuePair<string, Dictionary<string, decimal>> GetPredicted(Dictionary<string, Dictionary<string, decimal>> actual)
        {
            return actual.First(w => w.Key.Contains("12") && w.Key.Contains("104") && w.Key.Contains("0.001"));
        }

        [Then(@"Total Trades should be (.*)")]
        public void ThenTotalTradesShouldBe(int p0)
        {
            var actual = GetResults();
            var predicted = GetPredicted(actual);
            Assert.AreEqual(p0, predicted.Value["TotalNumberOfTrades"]);
        }

        [Then(@"last run should produce different result")]
        public void ThenSecondThreadShouldHaveResult()
        {
            var actual = GetResults();
            if (ScenarioContext.Current.Get<OptimizerConfiguration>().MaxThreads > 1)
            {
                Assert.True(actual.Count > 1);

                //probabilistic assert that given random input, expect more than half to produce a different result
                var percentUnique = (double)actual.Select(s => s.Value["SharpeRatio"]).Distinct().Count() / (double)ScenarioContext.Current.Get<OptimizerConfiguration>().MaxThreads;
                Assert.GreaterOrEqual(percentUnique, 0.5);
            }
        }

        [Then(@"multiple threads should execute in parallel")]
        public void ThenSecondThreadShouldExecuteInParallel()
        {
            int maxThreads = ScenarioContext.Current.Get<OptimizerConfiguration>().MaxThreads;
            if (maxThreads == 1)
            {
                return;
            }
            string[] log = null;

            SpinWait.SpinUntil(() =>
            {
                try
                {
                    log = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trace.txt"));
                    return log?.Count() == maxThreads * 2;
                }
                catch (Exception)
                {
                    return false;
                }
            },
            TimeSpan.FromSeconds(10));

            //todo: flaky asserts due to missing log entries
            for (int i = 0; i < maxThreads; i++)
            {
                log[i].EndsWith("started.");
            }
            for (int i = maxThreads; i < maxThreads * 2; i++)
            {
                log[i].EndsWith("finished.");
            }
        }

        private Dictionary<string, Dictionary<string, decimal>> GetResults()
        {
            if (ScenarioContext.Current.Get<OptimizerConfiguration>().UseSharedAppDomain)
            {
                return SingleAppDomainManager.GetResults();
            }
            else
            {
                return LegacyAppDomainManager.Instance.GetResults();
            }
        }

    }
}
