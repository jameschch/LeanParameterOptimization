using GeneticSharp.Domain.Chromosomes;
using Jtc.Optimization.LeanOptimizer.Base;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using SharpLearning.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jtc.Optimization.LeanOptimizer
{

    public class SharpeMaximizer : OptimizerFitness
    {

        public virtual string ScoreKey { get; set; } = "SharpeRatio";
        public override string Name { get; set; } = "Sharpe";
        public virtual IChromosome Best { get; set; }
        private ConditionalWeakTable<OptimizerResult, string> _resultIndex;
        protected static object Locker = new object();
        private const double ErrorFitness = 1.01;
        public int Seed { get; set; } = 42;
        protected static bool HasRunActual {get; set; } = false;

        public SharpeMaximizer(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
        {
            _resultIndex = new ConditionalWeakTable<OptimizerResult, string>();
        }

        public override double Evaluate(IChromosome chromosome)
        {
            try
            {
                var parameters = Config.Genes.Select(s =>
                    new MinMaxParameterSpec(min: s.Min ?? s.Actual.Value, max: s.Max ?? s.Actual.Value,
                    transform: Transform.Linear, parameterType: s.Precision > 0 ? ParameterType.Continuous : ParameterType.Discrete)
                ).ToArray();


                IOptimizer optimizer = null;
                if (Config.Fitness != null)
                {
                    if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.RandomSearch.ToString())
                    {
                        optimizer = new RandomSearchOptimizer(parameters, iterations: Config.Generations, seed: Seed, maxDegreeOfParallelism: Config.MaxThreads);
                    }
                    else if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.ParticleSwarm.ToString())
                    {
                        optimizer = new ParticleSwarmOptimizer(parameters, maxIterations: Config.Generations, numberOfParticles: Config.PopulationSize,
                            seed: Seed, maxDegreeOfParallelism: Config.MaxThreads);
                    }
                    else if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.Bayesian.ToString())
                    {
                        optimizer = new BayesianOptimizer(parameters: parameters, iterations: Config.Generations, randomStartingPointCount: Config.PopulationSize,
                            functionEvaluationsPerIterationCount: Config.PopulationSize, seed: Seed);
                        //optimizer = new BayesianOptimizer(parameters, iterations: Config.Generations, randomStartingPointCount: Config.PopulationSize,
                        //    functionEvaluationsPerIteration: Config.MaxThreads, seed: 42, maxDegreeOfParallelism: Config.MaxThreads, allowMultipleEvaluations: true);
                    }
                    else if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.GlobalizedBoundedNelderMead.ToString())
                    {
                        optimizer = new GlobalizedBoundedNelderMeadOptimizer(parameters, maxRestarts: Config.Generations,
                            maxIterationsPrRestart: Config.PopulationSize, seed: Seed, maxDegreeOfParallelism: Config.MaxThreads);
                    }
                    else if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.Smac.ToString())
                    {
                        optimizer = new SmacOptimizer(parameters, iterations: Config.Generations, randomStartingPointCount: Config.PopulationSize, seed: 42, 
                            functionEvaluationsPerIterationCount: Config.MaxThreads);
                    }
                    else if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.GridSearch.ToString())
                    {
                        optimizer = new GridSearchOptimizer(parameters);
                    }
                    else if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.Genetic.ToString())
                    {
                        throw new Exception("Genetic optimizer cannot be used with Sharpe Maximizer");
                    }
                }
                else
                {
                    throw new ArgumentException("No fitness section was configured.");
                }


                Func<double[], OptimizerResult> minimize = p => Minimize(p, (Chromosome)chromosome);

                var result = optimizer.OptimizeBest(minimize);

                Best = MergeFromResult(result, chromosome);

                return result.Error;
            }
            catch (Exception ex)
            {
                LogProvider.ErrorLogger.Error(ex);
                return ErrorFitness;
            }
        }

        public virtual OptimizerResult Minimize(double[] p, Chromosome configChromosome)
        {
            var id = Guid.NewGuid().ToString("N");
            try
            {
                StringBuilder output = new StringBuilder();
                var list = configChromosome.ToDictionary();

                list.Add("Id", id);
                output.Append("Id: " + id + ", ");

                var isActual = false;
                lock (Locker)
                {
                    isActual = !HasRunActual ? true : false;
                    HasRunActual = true;
                }

                for (int i = 0; i < Config.Genes.Count(); i++)
                {
                    var key = Config.Genes.ElementAt(i).Key;
                    var precision = Config.Genes.ElementAt(i).Precision ?? 0;

                    if (isActual)
                    {
                        p[i] = Config.Genes[i].Actual ?? p[i];
                    }

                    var value = Math.Round(p[i], precision);
                    list[key] = value;

                    output.Append(key + ": " + value.ToString() + ", ");
                }
                isActual = false;

                if (Config.StartDate.HasValue && Config.EndDate.HasValue)
                {
                    output.AppendFormat("Start: {0}, End: {1}, ", Config.StartDate, Config.EndDate);
                }

                var score = GetScore(list, Config);
                var fitness = CalculateFitness(score);

                output.AppendFormat("{0}: {1}", Name, fitness.Value.ToString("0.##"));
                LogProvider.OptimizerLogger.Info(output);

                var result = new OptimizerResult(p, fitness.Fitness);
                _resultIndex.Add(result, id);
                return result;
            }
            catch (Exception ex)
            {
                LogProvider.ErrorLogger.Error(ex, $"Id: {id}, Iteration failed.");

                var result = new OptimizerResult(p, ErrorFitness);
                _resultIndex.Add(result, id);
                return result;
            }
        }

        public virtual Dictionary<string, decimal> GetScore(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            return RunAlgorithm(list, config);
        }

        public virtual Dictionary<string, decimal> RunAlgorithm(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            if (config.UseSharedAppDomain)
            {
                return SingleContextIsolator.Instance.RunAlgorithm(list, config);
            }
            else
            {
                return MultipleContextIsolator.Instance.RunAlgorithm(list, config);
            }
        }

        private IChromosome MergeFromResult(OptimizerResult result, IChromosome source)
        {
            //todo: shouldn't this be a clone?
            var destination = (Chromosome)source;
            destination.Id = _resultIndex.GetValue(result, (k) => Guid.NewGuid().ToString("N"));

            var list = destination.ToDictionary();
            for (int i = 0; i < Config.Genes.Count(); i++)
            {
                var pair = (KeyValuePair<string, object>)destination.GetGene(i).Value;
                destination.ReplaceGene(i, new Gene(new KeyValuePair<string, object>(pair.Key, result.ParameterSet[i])));
            }

            destination.Fitness = result.Error;
            return destination;
        }

        protected override FitnessResult CalculateFitness(Dictionary<string, decimal> result)
        {
            var ratio = result[ScoreKey];

            if (Filter != null && !Filter.IsSuccess(result, this))
            {
                ratio = ErrorRatio;
            }

            return new FitnessResult
            {
                Value = ratio,
                Fitness = 1 - ((double)ratio / 1000)
            };
        }

        public override double GetValueFromFitness(double? fitness)
        {
            return ((fitness ?? ErrorFitness) - 1) * 1000 * -1;
        }

    }
}
