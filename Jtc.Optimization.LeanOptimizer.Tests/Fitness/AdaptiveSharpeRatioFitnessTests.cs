using GeneticSharp.Domain.Chromosomes;
using Jtc.Optimization.LeanOptimizer;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer.Tests
{
    [TestFixture()]
    public class AdaptiveSharpeRatioFitnessTests
    {
        private OptimizerConfiguration _config;
        private Wrapper _unit;


        [SetUp]
        public void SetUp()
        {

            _config = new OptimizerConfiguration
            {
                StartDate = new DateTime(2001, 1, 2),

                EndDate = new DateTime(2001, 1, 3)
            };
            _unit = new Wrapper(_config, Mock.Of<IFitnessFilter>());
        }


        [Test()]
        public void EvaluateTest()
        {
            var originalHours = CurrentHours(_config);
            ResultMediator.SetResults(AppDomain.CurrentDomain, new Dictionary<string, Dictionary<string, decimal>>());

            ResultMediator.GetResults(AppDomain.CurrentDomain).Add("key", new Dictionary<string, decimal>() { { "SharpeRatio", 123m } });

            //will not adapt on first result
            Assert.AreEqual(originalHours, CurrentHours(_config));
            _unit.Evaluate(Mock.Of<IChromosome>());
            Assert.AreEqual(originalHours, CurrentHours(_config));

            var fitness = _unit.Evaluate(Mock.Of<IChromosome>());

            var actualHours = CurrentHours(_config);

            //sharpe improved by 50%, period window should increase by 24 hours

            Assert.AreEqual(72, Math.Round(actualHours));

            actualHours = CurrentHours(_config);
            fitness = _unit.Evaluate(Mock.Of<IChromosome>());
            actualHours = CurrentHours(_config);

            //50% again

            Assert.AreEqual(108, Math.Round(actualHours));
        }

        [Test()]
        public void ExtendFailureKeysTest()
        {

            var failure = new Dictionary<string, decimal> { { "SharpeRatio", -10m } };
            var success = new Dictionary<string, decimal> { { "SharpeRatio", 1.5m } };
            var extending = new DateTime(2001, 1, 1);

            var failureKey = JsonConvert.SerializeObject(new Dictionary<string, object> { { "startDate", _config.StartDate }, { "period", 123 } });
            var successKey = JsonConvert.SerializeObject(new Dictionary<string, object> { { "startDate", _config.StartDate }, { "period", 456 } });
            var expectedKey = JsonConvert.SerializeObject(new Dictionary<string, object> { { "startDate", extending }, { "period", 123 } });

            ResultMediator.GetResults(AppDomain.CurrentDomain).Add(failureKey, failure);
            ResultMediator.GetResults(AppDomain.CurrentDomain).Add(successKey, success);

            _unit.ExtendFailureKeysWrapper(extending);

            Assert.AreEqual(1.5m, ResultMediator.GetResults(AppDomain.CurrentDomain)[successKey]["SharpeRatio"]);
            Assert.AreEqual(-10m, ResultMediator.GetResults(AppDomain.CurrentDomain)[expectedKey]["SharpeRatio"]);
        }

        private double CurrentHours(OptimizerConfiguration config)
        {
            return config.EndDate.Value.AddDays(1).AddTicks(-1).Subtract(config.StartDate.Value).TotalHours;
        }

        private class Wrapper : AdaptiveSharpeRatioFitness
        {
            double _previous = 0.5;

            public Wrapper(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
            {
            }

            protected override double EvaluateBase(IChromosome chromosome)
            {
                _previous += _previous * 0.5;
                return _previous;
            }

            public void ExtendFailureKeysWrapper(DateTime extending)
            {
                ExtendFailureKeys(extending);
            }
        }

    }
}