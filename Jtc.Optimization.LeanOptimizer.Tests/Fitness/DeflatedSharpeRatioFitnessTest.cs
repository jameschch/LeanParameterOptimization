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

    [TestFixture]
    public class DeflatedSharpeRatioFitnessTest
    {

        [Test]
        public void CalculateExpectedMaximumTest()
        {
            var unit = new Wrapper(null);
            unit.Initialize();
            var actual = unit.CalculateExpectedMaximum();

            Assert.AreEqual(0.1132, actual, 0.0002);
        }

        [Test]
        public void CalculateDeflatedSharpeRatioTest()
        {
            var unit = new Wrapper(null);
            unit.Initialize();
            var actual = unit.CalculateDeflatedSharpeRatio(0.1132);

            Assert.AreEqual(0.9004, actual, 0.0002);
        }

        private class Wrapper : DeflatedSharpeRatioFitness
        {

            public Wrapper(IOptimizerConfiguration config) : base(config, null)
            {
            }

            public override void Initialize()
            {
                N = 100;
                V = 2;
                T = 1250;
                Skewness = -3;
                Kurtosis = 10;
                CurrentSharpeRatio = 2.5;
            }

        }
    }

}
