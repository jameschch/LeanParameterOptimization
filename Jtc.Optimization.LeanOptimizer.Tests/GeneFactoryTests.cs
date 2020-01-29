using GeneticSharp.Domain.Randomizations;
using Jtc.Optimization.Objects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer.Tests
{
    [TestFixture()]
    public class GeneFactoryTests
    {

        [SetUp]
        public void Setup()
        { 
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void InitializeTest()
        {
            GeneFactory.Initialize(new GeneConfiguration[0]);
            Assert.IsNotNull(GeneFactory.Config);
        }

        [Test()]
        public void RandomBetweenTest()
        {
            var actual = GeneFactory.RandomBetween(0, 1);
            Assert.IsTrue(actual < 2);
        }

        [Test()]
        public void RandomBetweenCanBeMaximumTest()
        {
            bool canBeMaximum = false;
            for (var i = 0; i < 10000; i++)
            {
                var actual = GeneFactory.RandomBetween(0, 1);
                if (actual == 1m) { canBeMaximum = true; break;  }
            }

            Assert.IsTrue(canBeMaximum);
        }

        [Test()]
        public void RandomBetweenPrecisionTest()
        {
            var actual = GeneFactory.RandomBetween(1.1, 1.2, 1);
            Assert.IsTrue(actual >= 1.1 && actual <= 1.2);
        }

        [Test()]
        public void RandomBetweenPrecisionCanBeMaximumTest()
        {
            bool canBeMaximum = false;
            for (var i = 0;i < 10000;i++)
            {
                var actual = GeneFactory.RandomBetween(1.1, 1.2, 1);
                if (actual == 1.2) { canBeMaximum = true; break; }
            }

            Assert.IsTrue(canBeMaximum);
        }

        [Test()]
        public void RandomBetweenPrecisionCanBeNegativeTest()
        {
            bool canBeNegative = false;
            for (var i = 0; i < 10000; i++)
            {
                var actual = GeneFactory.RandomBetween(-1, 1, 0);
                if (actual == -1) { canBeNegative = true; break; }
            }

            Assert.IsTrue(canBeNegative);
        }

        [Test()]
        public void GenerateTest()
        {
            var config = new[] { new GeneConfiguration { Key = "slow", Actual = 200 }, new GeneConfiguration { Key = "take", Precision = 2, Max = 0.06,
                Min = 0.04, Actual = 0.05 } };

            RandomizationProvider.Current = new BasicRandomization();
            GeneFactory.Initialize(config);

            var actual = GeneFactory.Generate(config[0], true);
            Assert.AreEqual(200, (double)((KeyValuePair<string, object>)actual.Value).Value);

            RandomizationProvider.Current = new BasicRandomization();
            actual = GeneFactory.Generate(config[1], false);

            Assert.IsTrue(double.TryParse(((KeyValuePair<string, object>)actual.Value).Value.ToString(), out var parsed));
            Assert.AreEqual(2, GeneFactory.GetPrecision(parsed));

        }
    }
}