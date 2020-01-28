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
            //if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Jtc.Optimization.LeanOptimizer.Example.dll")))
            //{
            //    File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Jtc.Optimization.LeanOptimizer.Example.dll"), 
            //        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Jtc.Optimization.LeanOptimizer.Example.dll"));
            //}

            SpinWait.SpinUntil(() =>
            {
                try
                {
                    File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trace.txt"));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            },
            TimeSpan.FromSeconds(1));



            Config.Set("algorithm-location", "../Optimization.Example/bin/debug/Jtc.Optimization.LeanOptimizer.Example.dll");

            var config = ScenarioContext.Current.Get<OptimizerConfiguration>();

            if (config.UseSharedAppDomain)
            {
                SingleAppDomainManager.Initialize();
                config.AlgorithmTypeName = "ParameterizedSharedAppDomainAlgorithm";
            }
            else
            {
                OptimizerAppDomainManager.Initialize();
            }

            var manager = new MaximizerManager();

            manager.Initialize(config, new SharpeMaximizer(config, new FitnessFilter()));
            manager.Start();
        }

    }
}
