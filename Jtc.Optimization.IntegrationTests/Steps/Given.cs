using Jtc.Optimization.LeanOptimizer;
using Jtc.Optimization.LeanOptimizer.Fitness;
using Jtc.Optimization.Objects;
using Newtonsoft.Json;
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
    public class Given
    {


        [Given(@"I have an optimization\.config")]
        public void GivenIHaveAnOptimization_Config()
        {
            //cleanup trace file
            SpinWait.SpinUntil(() =>
            {
                try
                {
                    var stream = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trace.txt"));
                    stream.Dispose();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            },
            TimeSpan.FromSeconds(1));

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var text = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "optimization_test.json"));
            var config = JsonConvert.DeserializeObject<OptimizerConfiguration>(text);
            ScenarioContext.Current.Set<OptimizerConfiguration>(config);
        }

        [Given(@"I have set maxThreads and generations to (.*)")]
        public void GivenIHaveSetMaxThreadsTo(int p0)
        {
            ScenarioContext.Current.Get<OptimizerConfiguration>().MaxThreads = p0;
            ScenarioContext.Current.Get<OptimizerConfiguration>().Generations = p0;
        }

        [Given(@"I have set useSharedAppDomain to (.*)")]
        public void GivenIHaveSetUseSharedAppDomainToTrue(bool useSharedAppDomain)
        {
            ScenarioContext.Current.Get<OptimizerConfiguration>().UseSharedAppDomain = useSharedAppDomain;
        }

        [Given(@"I have set Fitness to WalkForwardSharpeMaximizer with folds (.*)")]
        public void GivenIHaveSetFitnessToWalkForwardSharpeMaximizerWithFolds(int p0)
        {
            ScenarioContext.Current.Get<OptimizerConfiguration>().FitnessTypeName = typeof(WalkForwardSharpeMaximizer).FullName;
            ScenarioContext.Current.Get<OptimizerConfiguration>().Fitness = new FitnessConfiguration
            {
                Folds = p0,
                OptimizerTypeName = "RandomSearch"
            };
        }


    }
}
