using DeepEqual.Syntax;
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
    public class WalkForwardSharpeMaximizerTests
    {
        //todo: actual with precision
        [TestCase("2001-01-01", "2002-07-02", 2,
            new[]
            {
                "2001-01-01", "2001-10-01", "2001-10-02", "2002-02-16",
                "2001-05-18", "2002-02-15", "2002-02-16", "2002-07-02"
            },
            new[]
            {
                0d, 0d, 1d, 1d, 1d, 1d, 2d, 2d
            },
            Description = "9 months, 2 folds")
        ]
        [TestCase("2001-01-01", "2001-01-06", 3,
            new[] 
            {
            "2001-01-01", "2001-01-02", "2001-01-03", "2001-01-04",
            "2001-01-02", "2001-01-03", "2001-01-04", "2001-01-05",
            "2001-01-03", "2001-01-04", "2001-01-05", "2001-01-06"
            },
            new[]
            {
                0d, 0d, 1d, 1d, 1d, 1d, 2d, 2d, 2d, 2d, 3d, 3d
            },
            Description = "6 days, 3 folds")
        ]
        public void EvaluateTest(DateTime startDate, DateTime endDate, int folds, string[] expectedDates, double[] expectedValues)
        {

            var config = new OptimizerConfiguration
            {
                StartDate = startDate,
                EndDate = endDate,
                FitnessTypeName = "Optimization.OptimizerFitness",
                EnableFitnessFilter = true,
                Genes = SetupGenes(),
                Fitness = new FitnessConfiguration { Folds = folds }
            };

            var unit = new Wrapper(config);

            GeneFactory.Initialize(config.Genes);

            var actual = unit.Evaluate(new Chromosome(true, config.Genes));

            Assert.AreEqual(1, actual);

            var actualDates = new List<DateTime>();
            var actualValues = new List<double>();


            for (int i = 0; i < unit.ActualInSampleConfig.Count(); i++)
            {
                actualDates.Add(unit.ActualInSampleConfig[i].StartDate.Value);
                actualDates.Add(unit.ActualInSampleConfig[i].EndDate.Value);
                actualDates.Add(unit.ActualOutSampleConfig[i].StartDate.Value);
                actualDates.Add(unit.ActualOutSampleConfig[i].EndDate.Value);

                actualValues.Add((int)unit.ActualInSampleChromosome[i].ToDictionary()["Key"]);
                actualValues.Add((int)unit.ActualInSampleChromosome[i].ToDictionary()["Key2"]);
                actualValues.Add((int)unit.ActualOutSampleList[i]["Key"]);
                actualValues.Add((int)unit.ActualOutSampleList[i]["Key2"]);
            }

            expectedDates.Select(s => DateTime.Parse(s)).ShouldDeepEqual(actualDates);
            expectedValues.ShouldDeepEqual(actualValues);
        }

        public static GeneConfiguration[] SetupGenes()
        {
            return new[] { new GeneConfiguration { Key = "Key", Min = 0, Max = 10.0, Actual = 0  }, new GeneConfiguration { Key = "Key2", Min = 0, Max = 10.0, Actual = 0 } };
        }

        private class Wrapper : WalkForwardSharpeMaximizer
        {

            private double actualBest = 0;
            public List<Chromosome> ActualInSampleChromosome = new List<Chromosome>();
            private static Mock<ISharpeMaximizerFactory> _factory = new Mock<ISharpeMaximizerFactory>();
            public override ISharpeMaximizerFactory SharpeMaximizerFactory { get; } = _factory.Object;
            public List<Dictionary<string, object>> ActualOutSampleList { get; private set; } = new List<Dictionary<string, object>>();
            public List<IOptimizerConfiguration> ActualOutSampleConfig { get; private set; } = new List<IOptimizerConfiguration>();
            public List<IOptimizerConfiguration> ActualInSampleConfig { get; private set; } = new List<IOptimizerConfiguration>();

            public Wrapper(IOptimizerConfiguration config) : base(config, null)
            {
                //the in sample mock records the chromosome passed in and returns best genes with incremented actual values
                Mock<SharpeMaximizer> inSampleMaximizer = null;
                _factory.Setup(m => m.Create(It.IsAny<IOptimizerConfiguration>(), It.IsAny<IFitnessFilter>())).Returns<IOptimizerConfiguration, IFitnessFilter>((o, f) =>
                {
                    inSampleMaximizer = new Mock<SharpeMaximizer>(o, null);
                    inSampleMaximizer.Setup(m => m.Evaluate(It.IsAny<IChromosome>())).Callback<IChromosome>(c =>
                    {
                        ActualInSampleChromosome.Add((Chromosome)c);
                    }).Returns(1);

                    ActualInSampleConfig.Add(o);

                    inSampleMaximizer.Setup(m => m.Best).Returns(new Chromosome(true, SetupBestGenes()));

                    return inSampleMaximizer.Object;
                });

            }

            public override Dictionary<string, decimal> GetScore(Dictionary<string, object> list, IOptimizerConfiguration config)
            {
                ActualOutSampleList.Add(list);
                ActualOutSampleConfig.Add(config);
                return new Dictionary<string, decimal> { { "SharpeRatio", 1 }, { "CompoundingAnnualReturn", 1 }, { "TotalNumberOfTrades", 1 }, { "LossRate", 0.1m } };
            }

            private GeneConfiguration[] SetupBestGenes()
            {
                actualBest++;
                //increment best gene each iteration
                var best = SetupGenes();
                foreach (var item in best)
                {
                    item.Actual = actualBest;
                }

                return best;
            }

        }


    }
}