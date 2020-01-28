using Jtc.Optimization.LeanOptimizer;
using Jtc.Optimization.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace Jtc.Optimization.IntegrationTests.Steps
{
    [Binding]
    public class Then
    {
        [Then(@"the Sharp Ratio should be (.*)")]
        public void ThenTheSharpRatioShouldBe(double p0)
        {
            var actual = GetResults();
            Assert.Equal(p0, (double)actual.First().Value["SharpeRatio"], 3);
        }

        [Then(@"Total Trades should be (.*)")]
        public void ThenTotalTradesShouldBe(int p0)
        {
            var actual = GetResults();
            Assert.Equal(p0, actual.First().Value["TotalNumberOfTrades"]);
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
                Assert.True(percentUnique >= 0.5);
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
                return OptimizerAppDomainManager.GetResults();
            }
        }

    }
}
