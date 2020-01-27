using GeneticSharp.Domain.Chromosomes;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer.Tests
{
    [TestFixture()]
    public class OptimizerFitnessTests
    {

        Wrapper _unit;

        public OptimizerFitnessTests()
        {
            _unit = new Wrapper(new OptimizerConfiguration
            {
                FitnessTypeName = "Optimization.OptimizerFitness",
                EnableFitnessFilter = true
            }, new FitnessFilter());
        }

        [TestCase(1, 12, 0.22, 0.5)]
        [TestCase(-1, 12, 0, 0.5)]
        [TestCase(-1, 0, 0, 0.5)]
        [TestCase(1, 12, 0, 1)]
        public void CalculateFitnessTest(decimal car, int trades, double expected, decimal lossRate)
        {
            var actual = _unit.CalculateFitnessWrapper(new Dictionary<string, decimal> {
                { "SharpeRatio", 1 },
                { "CompoundingAnnualReturn", car },
                { "TotalNumberOfTrades", trades },
                { "LossRate", lossRate }
            });
            Assert.AreEqual(expected, actual.Item2);
        }

        [Test]
        public void EvaluateTest()
        {
            //todo:
            _unit.Evaluate(Mock.Of<IChromosome>());

        }

        private class Wrapper : OptimizerFitness
        {

            public Wrapper(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
            {
            }

            public Tuple<decimal, double> CalculateFitnessWrapper(Dictionary<string, decimal> result)
            {
                var fitness = base.CalculateFitness(result);
                return new Tuple<decimal, double>(fitness.Value, fitness.Fitness);
            }
        }

    }
}