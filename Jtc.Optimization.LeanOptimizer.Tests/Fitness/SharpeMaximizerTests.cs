using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
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
    public class SharpeMaximizerTests
    {

        [Test]
        public void MinimizeTest()
        {
            var genes = new[]
                {
                    new GeneConfiguration
                    {
                        Key = "1",
                        Actual = 1.23
                    },
                    new GeneConfiguration
                    {
                        Key = "2",
                        Actual = 123,
                        Min = 1,
                        Max = 123
                    },
                    new GeneConfiguration
                    {
                        Key = "3",
                        Min = 1,
                        Max = 2
                    }
                };

            var config = new OptimizerConfiguration
            {
                Genes = genes,
                FitnessTypeName = "Optimization.OptimizerFitness",
                EnableFitnessFilter = true,

            };

            var unit = new Mock<SharpeMaximizer>(config, new FitnessFilter()) { CallBase = true };

            unit.Setup(x => x.RunAlgorithm(It.IsAny<Dictionary<string, object>>(), It.IsAny<IOptimizerConfiguration>())).Returns<Dictionary<string, object>, IOptimizerConfiguration>((l, c) =>
            {
                return new Dictionary<string, decimal> { { "SharpeRatio", 1 }, { "CompoundingAnnualReturn", 1 }, { "TotalNumberOfTrades", 1 },
                    { "LossRate", 0.1m } };
            });

            GeneFactory.Initialize(config.Genes);
            RandomizationProvider.Current = new BasicRandomization();
            var chromosome = new Chromosome(true, genes, false);

            var actual = unit.Object.Minimize(new[] { 3.21, 321, 456 }, chromosome);

            Assert.AreEqual(0.999, actual.Error);
            Assert.AreEqual(1.23, actual.ParameterSet[0]);
            Assert.AreEqual(123, actual.ParameterSet[1]);
            Assert.AreEqual(456, actual.ParameterSet[2]);

            var nextActual = unit.Object.Minimize(new[] { 3.21, 321, 789 }, chromosome);

            Assert.AreEqual(0.999, nextActual.Error);
            Assert.AreEqual(3.21, nextActual.ParameterSet[0]);
            Assert.AreEqual(321, nextActual.ParameterSet[1]);
            Assert.AreEqual(789, nextActual.ParameterSet[2]);
        }

        //todo:
        [Test]
        public void EvaluateTest()
        { }

    }
}