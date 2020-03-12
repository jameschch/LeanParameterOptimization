using GeneticSharp.Domain.Chromosomes;
using Jtc.Optimization.LeanOptimizer.Fitness;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Jtc.Optimization.LeanOptimizer.Fitness.Tests
{
    [TestFixture()]
    public class WalkForwardWeightedMetricSharpeMaximizerTests
    {

        WalkForwardWeightedMetricSharpeMaximizer _unit;
        private OptimizerConfiguration _config;

        [SetUp]
        public void Setup()
        {
            _config = new OptimizerConfiguration { Genes = new[] { new GeneConfiguration { Key = "p1", Min = 1, Max = 2 }, new GeneConfiguration { Key = "p2", Min = 1, Max = 2 } } };
            IFitnessFilter filter = null;
            _unit = new WalkForwardWeightedMetricSharpeMaximizer(_config, filter);
            _unit.WalkForwardSharpeMaximizerFactory = new Factory();
        }

        [Test]
        public void GetScoreTest()
        {
            var actual = _unit.GetScore(null, _config);

            Assert.AreEqual(2, actual["AverageSharpe"]);
            Assert.AreEqual(2.225, actual["WalkForwardWeightedMetricSharpe"]);
        }


        private class Factory : IWalkForwardSharpeMaximizerFactory
        {
            public IWalkForwardSharpeMaximizer Create(IOptimizerConfiguration config, IFitnessFilter filter)
            {
                var mock = new Mock<IWalkForwardSharpeMaximizer>();
                mock.Setup(m => m.Evaluate(It.IsAny<IChromosome>())).Returns(2);
                var allBest = new List<Dictionary<string, object>> { new Dictionary<string, object> { { "p1", 0.1 }, { "p2", 10.0 } }, new Dictionary<string, object> { { "p1", 0.2 }, { "p2", 11.0 } } };
                mock.Setup(m => m.AllBest).Returns(allBest);
                var allScores = new List<FitnessResult> { new FitnessResult { Fitness = 1, Value = 2 }, new FitnessResult { Fitness = 2, Value = 3 } };
                mock.Setup(m => m.AllScores).Returns(allScores);

                return mock.Object;

            }
        }
    }
}