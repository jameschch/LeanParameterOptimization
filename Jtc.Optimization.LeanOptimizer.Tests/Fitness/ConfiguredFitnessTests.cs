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
    public class ConfiguredFitnessTests
    {

        Wrapper unit;

        [TestCase("TotalNumberOfTrades")]
        [TestCase("Total Trades")]
        public void CalculateFitnessTest(string key)
        {
            unit = new Wrapper(new OptimizerConfiguration
            {
                FitnessTypeName = "Optimization.ConfiguredFitness",
                Fitness = new FitnessConfiguration
                {
                    Scale = 0.1,
                    Modifier = -1,
                    Name = "TestName",
                    ResultKey = key
                }
            });

            var actual = unit.CalculateFitnessWrapper(new Dictionary<string, decimal> { { "TotalNumberOfTrades", 10 } });

            Assert.AreEqual(-1d, actual.Item2);
        }

        [Test()]
        public void GetValueFromFitnessTest()
        {
            unit = new Wrapper(new OptimizerConfiguration
            {
                FitnessTypeName = "Optimization.ConfiguredFitness",
                Fitness = new FitnessConfiguration
                {
                    Scale = 0.1,
                    Modifier = -1,
                    Name = "TestName",
                    ResultKey = "TotalTrades"
                }
            });

            var actual = unit.GetValueFromFitness(-1d);
            Assert.AreEqual(10, actual);
        }

        private class Wrapper : ConfiguredFitness
        {

            public Wrapper(IOptimizerConfiguration config) : base(config)
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