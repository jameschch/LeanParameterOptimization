using GeneticSharp.Domain.Chromosomes;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using SharpLearning.Optimization;
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
        private int _foldMaxThreads;
        private IWalkForwardSharpeMaximizer _maximizer;
        private static string[] excluding = new[] { "Id", "startDate", "endDate" };
        public IWalkForwardSharpeMaximizerFactory WalkForwardSharpeMaximizerFactory { get; set; }
        public override string Name { get; set; } = "WalkForwardWeightedMetricSharpe";

        public WalkForwardWeightedMetricSharpeMaximizer(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
        {
            //one thread for parent and max threads for fold optimizer
            _foldMaxThreads = config.MaxThreads;
            config.MaxThreads = 1;

            WalkForwardSharpeMaximizerFactory = new WalkForwardSharpeMaximizerFactory();
        }

        public override Dictionary<string, decimal> GetScore(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            config.MaxThreads = _foldMaxThreads;
            if (!string.IsNullOrEmpty(config.Fitness?.FoldOptimizerTypeName))
            {
                config.Fitness.OptimizerTypeName = config.Fitness.FoldOptimizerTypeName;
            }
            if (config.Fitness?.FoldGenerations.HasValue ?? false)
            {
                config.Generations = config.Fitness.FoldGenerations.Value;
            }

            _maximizer = WalkForwardSharpeMaximizerFactory.Create(config, Filter);

            foreach (var item in config.Genes)
            {
                item.Actual = (double)list[item.Key];
            }
            var chromosome = new Chromosome(true, config.Genes, false);

            LogProvider.GenerationsLogger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(chromosome.GetGenes()));

            //returns non-normalized average sharpe
            var score = _maximizer.Evaluate(chromosome);

            //parameter matrix
            var flattened = GetBestGenes().ToArray();

            var cost = WeightedMetricCost.Calculate(_maximizer.AllScores.Select(a => (double)a.Value).ToArray(), flattened);

            //if success, contrain future genes by min max range of current best
            if (score > -10)
            {
                var pairs = _maximizer.AllBest.SelectMany(s => s.Where(w => !excluding.Contains(w.Key)).Select(v => ConvertToDouble(v)));

                var minmax = pairs.GroupBy(g => g.Key).Select(s => new { Key = s.Key, Min = s.Min(m => m.Value), Max = s.Max(m => m.Value) });

                foreach (var item in minmax)
                {
                    var gene = Config.Genes.Single(g => g.Key == item.Key);
                    if (Math.Round(item.Max, gene.Precision ?? 0) == Math.Round(item.Min, gene.Precision ?? 0))
                    {
                        LogProvider.GenerationsLogger.Info($"{item.Key} parameter could not be constrained.");
                        continue;
                    }
                    gene.Max = item.Max;
                    gene.Min = item.Min;
                }
            }
            else
            {
                LogProvider.GenerationsLogger.Info($"All parameters could not be constrained.");
                throw new Exception("All folds failed on out-samples. ");
            }

            return new Dictionary<string, decimal>() { { Name, (decimal)cost }, { AverageSharpe, (decimal)score } };
        }

        private IEnumerable<double[]> GetBestGenes()
        {
            return _maximizer.AllBest.Select(s => s.Where(w => !excluding.Contains(w.Key))
                            .Select(v => ConvertToDouble(v)).Select(ss => ss.Value).ToArray());
        }

        private static KeyValuePair<string, double> ConvertToDouble(KeyValuePair<string, object> v)
        {
            return new KeyValuePair<string, double>(v.Key, v.Value is int ? (double)(int)v.Value : (double)v.Value);
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

        public override OptimizerResult Minimize(double[] p, Chromosome configChromosome)
        {
            var result = base.Minimize(p, configChromosome);
            var parameterSet = result.ParameterSet;

            var optimized = GetBestGenes().ElementAt(new Random().Next(0, _maximizer.AllBest.Count()));

            for (int i = 0; i < parameterSet.Length; i++)
            {
                parameterSet[i] = optimized[i];
            }
            //return the result of a random alpha
            return new OptimizerResult(parameterSet, result.Error);
        }

    }
}
