using GeneticSharp.Domain.Chromosomes;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.Optimization.LeanOptimizer
{
    public class WalkForwardWeightedMetricSharpeMaximizer : SharpeMaximizer
    {

        private const string AverageSharpe = "AverageSharpe";
        public static object IsActualLocker = new object();
        private bool _isActual;
        public IWalkForwardSharpeMaximizerFactory WalkForwardSharpeMaximizerFactory { get; set; }
        public override string Name { get; set; } = "WalkForwardWeightedMetricSharpe";

        public WalkForwardWeightedMetricSharpeMaximizer(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
        {
            WalkForwardSharpeMaximizerFactory = new WalkForwardSharpeMaximizerFactory();
        }

        public override Dictionary<string, decimal> GetScore(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            var maximizer = WalkForwardSharpeMaximizerFactory.Create(config, Filter);

            var chromosome = new Chromosome(true, config.Genes);
            foreach (var item in config.Genes)
            {
                item.Actual = (double)list[item.Key];
            }

            //returns non-normalized average sharpe
            var score = maximizer.Evaluate(chromosome);

            var excluding = new[] { "Id", "startDate", "endDate" };
            //parameter matrix
            var flattened = maximizer.AllBest.Select(s => s.Where(w => !excluding.Contains(w.Key))
                .Select(v => v.Value is int ? (double)(int)v.Value : (double)v.Value).ToArray()).ToArray();

            var cost = WeightedMetricCost.Calculate(maximizer.AllScores.Select(a => (double)a.Value).ToArray(), flattened);

            return new Dictionary<string, decimal>() { { Name, (decimal)cost }, { AverageSharpe, (decimal)score } };
        }

        protected override FitnessResult CalculateFitness(Dictionary<string, decimal> result)
        {
            var cost = (double)result[Name];
            var fitness = new FitnessResult { Fitness = cost, Value = result[AverageSharpe] };

            var output = new StringBuilder();
            output.AppendFormat("{0}: {1}, ", AverageSharpe, fitness.Value.ToString("0.##"));
            output.AppendFormat("{0}: {1}", Name, fitness.Fitness.ToString("0.##"));
            LogProvider.GenerationsLogger.Info(output);

            return fitness;
        }

        private bool GetIsActual()
        {
            lock (IsActualLocker)
            {
                if (!_isActual)
                {
                    return _isActual;
                }
                else
                {
                    _isActual = false;
                    return _isActual;
                }
            }
        }

    }
}
