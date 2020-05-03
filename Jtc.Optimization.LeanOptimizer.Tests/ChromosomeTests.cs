using NUnit.Framework;
using Jtc.Optimization.LeanOptimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtc.Optimization.Objects;

namespace Jtc.Optimization.LeanOptimizer.Tests
{
    [TestFixture()]
    public class ChromosomeTests
    {

        [TestCase(false, 1, false)]
        [TestCase(false, 0, true)]
        [TestCase(true, 2, false)]
        [TestCase(true, 1, true)]
        public void ValidateLengthTest(bool isGenetic, int length, bool hasException)
        {
            var config = new List<GeneConfiguration>();
            for (int i = 0; i < length; i++)
            {
                config.Add(new GeneConfiguration { Min = 1, Max = 2 });
            }

            if (hasException)
            {
                Assert.Throws<ArgumentException>(() => new Chromosome(false, config.ToArray(), isGenetic));
            }
            else
            {
                new Chromosome(false, config.ToArray(), isGenetic);
            }
        }


        [Test()]
        public void GenerateGeneTest()
        {
        }

        [Test()]
        public void CreateNewTest()
        {
        }

        [Test()]
        public void CloneTest()
        {
        }

        [Test()]
        public void ToDictionaryTest()
        {
        }

        [Test()]
        public void ToKeyValueStringTest()
        {
        }
    }
}