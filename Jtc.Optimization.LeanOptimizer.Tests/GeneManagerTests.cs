using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
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
    public class GeneManagerTests
    {
        GeneConfiguration[] genes;

        [SetUp]
        public void Setup()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            RandomizationProvider.Current = new BasicRandomization();
            genes = new[] { new GeneConfiguration { Key = "abc", Min = 1, Max = 3 }, new GeneConfiguration { Key = "def", Min = 1, Max = 3 } };
        }

        [Test()]
        public void StartTest()
        {
            var config = new Mock<IOptimizerConfiguration>();
            config.Setup(c => c.PopulationSize).Returns(2);
            config.Setup(c => c.Genes).Returns(genes);

            var fitness = new Mock<OptimizerFitness>(config.Object, null);
            fitness.Setup(f => f.Evaluate(It.IsAny<IChromosome>())).Returns(-10).Verifiable();
            var unit = new GeneManager();
            unit.Initialize(config.Object, fitness.Object);
            unit.Start();
            fitness.Verify();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void StartActualTest(bool useActualGenesForWholeGeneration)
        {
            var config = new Mock<IOptimizerConfiguration>();
            config.Setup(c => c.PopulationSize).Returns(2);
            int actualValue = 4;
            genes.First().Actual = actualValue;
            config.Setup(c => c.Genes).Returns(genes);
            config.Setup(c => c.UseActualGenesForWholeGeneration).Returns(useActualGenesForWholeGeneration);

            var fitness = new Mock<OptimizerFitness>(config.Object, null);
            List<IChromosome> actualChromose = new List<IChromosome>();
            fitness.Setup(f => f.Evaluate(It.IsAny<IChromosome>())).Returns(-10).Callback<IChromosome>(c => actualChromose.Add(c));
            var unit = new GeneManager();
            unit.Initialize(config.Object, fitness.Object);
            unit.Start();

            fitness.Verify();
            CollectionAssert.IsNotEmpty(actualChromose.Where(a => (int)((KeyValuePair<string, object>)a.GetGenes().First().Value).Value == actualValue));

            var actualCount = actualChromose.Where(a => (int)((KeyValuePair<string, object>)a.GetGenes().First().Value).Value == actualValue).Count();
            Assert.AreEqual(useActualGenesForWholeGeneration ? 2 : 1, actualCount);

        }

    }
}