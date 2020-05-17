using Jtc.Optimization.LeanOptimizer;
using Jtc.Optimization.Objects;
using QuantConnect.Configuration;
using System;
using System.Diagnostics;
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
            var config = ScenarioContext.Current.Get<OptimizerConfiguration>();

            if (config.UseSharedAppDomain)
            {
                config.AlgorithmTypeName = "ParameterizedSharedAppDomainAlgorithm";
            }

            var manager = new MaximizerManager();

            manager.Initialize(config, new SharpeMaximizer(config, new FitnessFilter()));
            manager.Start();
        }

        [When(@"I run the Launcher executable")]
        public void WhenIRunTheLauncherExecutable()
        {
            var resultfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "optimizer.txt");
            if (File.Exists(resultfile))
            {
                File.Delete(resultfile);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Jtc.Optimization.Launcher.Legacy.exe"),
                CreateNoWindow = true
            };
            var process = Process.Start(startInfo);

            process.WaitForExit(10000);
            process.Kill();
        }
    }
}
