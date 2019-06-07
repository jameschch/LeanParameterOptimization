using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;
using SharpLearning.Optimization;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Optimization
{

    public class SharpeMaximizer : OptimizerFitness
    {

        public virtual string ScoreKey { get; set; } = "SharpeRatio";
        public override string Name { get; set; } = "Sharpe";
        public IChromosome Best { get; set; }
        private ConditionalWeakTable<OptimizerResult, string> _resultIndex;
        private const double ErrorFitness = 1.01;

        public SharpeMaximizer(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
        {
            _resultIndex = new ConditionalWeakTable<OptimizerResult, string>();
        }

        public override double Evaluate(IChromosome chromosome)
        {
            try
            {
                var parameters = Config.Genes.Select(s =>
                    new MinMaxParameterSpec(min: (double)(s.MinDecimal ?? s.MinInt.Value), max: (double)(s.MaxDecimal ?? s.MaxInt.Value),
                    transform: Transform.Linear, parameterType: s.Precision > 0 ? ParameterType.Continuous : ParameterType.Discrete)
                ).ToArray();


                IOptimizer optimizer = null;
                if (Config.Fitness != null)
                {
                    if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.RandomSearch.ToString())
                    {
                        optimizer = new RandomSearchOptimizer(parameters, iterations: Config.Generations, seed: 42, maxDegreeOfParallelism: Config.MaxThreads);
                    }
                    else if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.ParticleSwarm.ToString())
                    {
                        optimizer = new ParticleSwarmOptimizer(parameters, maxIterations: Config.Generations, numberOfParticles: Config.PopulationSize,
                            seed: 42, maxDegreeOfParallelism: Config.MaxThreads);
                    }
                    else if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.Bayesian.ToString())
                    {
                        optimizer = new BayesianOptimizer(parameters, maxIterations: Config.Generations, numberOfStartingPoints: Config.PopulationSize, seed: 42);
                    }
                    else if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.GlobalizedBoundedNelderMead.ToString())
                    {
                        optimizer = new GlobalizedBoundedNelderMeadOptimizer(parameters, maxRestarts: Config.Generations,
                            maxIterationsPrRestart: Config.PopulationSize, seed: 42, maxDegreeOfParallelism: Config.MaxThreads);
                    }
                    else if (Config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.Genetic.ToString())
                    {
                        throw new Exception("Genetic optimizer cannot be used with Sharpe Maximizer");
                    }
                }

                //todo:
                // GridSearchOptimizer?

                Func<double[], OptimizerResult> minimize = p => Minimize(p, (Chromosome)chromosome);

                // run optimizer
                var result = optimizer.OptimizeBest(minimize);

                Best = ToChromosome(result, chromosome);

                return result.Error;
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                return ErrorFitness;
            }
        }

        protected OptimizerResult Minimize(double[] p, Chromosome configChromosome)
        {
            var id = Guid.NewGuid().ToString("N");
            try
            {
                StringBuilder output = new StringBuilder();
                var list = configChromosome.ToDictionary();

                list.Add("Id", id);
                output.Append("Id: " + id + ", ");

                for (int i = 0; i < Config.Genes.Count(); i++)
                {
                    var key = Config.Genes.ElementAt(i).Key;
                    var precision = Config.Genes.ElementAt(i).Precision ?? 0;
                    var value = Math.Round(p[i], precision);
                    list[key] = value;

                    output.Append(key + ": " + value.ToString() + ", ");
                }

                if (Config.StartDate.HasValue && Config.EndDate.HasValue)
                {
                    output.AppendFormat("Start: {0}, End: {1}, ", Config.StartDate, Config.EndDate);
                }

                var score = GetScore(list, Config);
                var fitness = CalculateFitness(score);

                output.AppendFormat("{0}: {1}", Name, fitness.Value.ToString("0.##"));
                Program.Logger.Info(output);

                var result = new OptimizerResult(p, fitness.Fitness);
                _resultIndex.Add(result, id);
                return result;
            }
            catch (Exception)
            {
                Program.Logger.Error($"Id: {id}, Iteration failed.");

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
                return SingleAppDomainManager.RunAlgorithm(list, config);
            }
            else
            {
                return OptimizerAppDomainManager.RunAlgorithm(list, config);
            }
        }

        private IChromosome ToChromosome(OptimizerResult result, IChromosome source)
        {
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
