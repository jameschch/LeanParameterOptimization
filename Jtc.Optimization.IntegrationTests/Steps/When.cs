using Jtc.Optimization.LeanOptimizer;
using Jtc.Optimization.Objects;
using QuantConnect.Configuration;
using System;
using System.IO;
using System.Threading;
using TechTalk.SpecFlow;

namespace Jtc.Optimization.IntegrationTests.Steps
{
    [Binding]
    public class When
    {

        [When(@"I optimize")]
        public void WhenIOptimize()
        {

            Config.Set("algorithm-location", "../Optimization.Example/bin/debug/Jtc.Optimization.LeanOptimizer.Example.dll");

            var config = ScenarioContext.Current.Get<OptimizerConfiguration>();

            if (config.UseSharedAppDomain)
            {
                config.AlgorithmTypeName = "ParameterizedSharedAppDomainAlgorithm";
            }

            var manager = new MaximizerManager();

            manager.Initialize(config, new SharpeMaximizer(config, new FitnessFilter()));
            manager.Start();
        }

    }
}
