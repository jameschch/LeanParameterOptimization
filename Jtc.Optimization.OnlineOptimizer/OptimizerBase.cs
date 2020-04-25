using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using Jtc.Optimization.Transformation;
using SharpLearning.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jtc.Optimization.OnlineOptimizer
{

    public abstract class OptimizerBase
    {
        protected IEnumerable<string> Keys { get; set; }
        private bool IsMaximizing { get; set; }
        protected IActivityLogger ActivityLogger { get; set; }
        public string Code { get; private set; }
        protected CancellationToken CancellationToken { get; private set; }

        public void Initialize(string code, IActivityLogger activityLogger)
        {
            Code = code;
            ActivityLogger = activityLogger;
        }

        public async Task<IterationResult> Start(IOptimizerConfiguration config, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;

            var parameters = config.Genes.Select(s =>
                new MinMaxParameterSpec(min: s.Min ?? s.Actual.Value, max: s.Max ?? s.Actual.Value,
                transform: Transform.Linear, parameterType: s.Precision > 0 ? ParameterType.Continuous : ParameterType.Discrete)
                ).ToArray();

            Keys = config.Genes.Where(g => g.Key != "id").Select(s => s.Key);

            IOptimizer optimizerMethod = null;
            if (config.Fitness != null)
            {
                if (config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.RandomSearch.ToString())
                {
                    optimizerMethod = new RandomSearchOptimizer(parameters, iterations: config.Generations, seed: 42, runParallel: false);
                }
                else if (config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.ParticleSwarm.ToString())
                {
                    optimizerMethod = new ParticleSwarmOptimizer(parameters, maxIterations: config.Generations, numberOfParticles: config.PopulationSize,
                        seed: 42, maxDegreeOfParallelism: 1);
                }
                else if (config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.Bayesian.ToString())
                {
                    optimizerMethod = new BayesianOptimizer(parameters: parameters, iterations: config.Generations, randomStartingPointCount: config.PopulationSize,
                        functionEvaluationsPerIterationCount: config.PopulationSize, seed: 42, runParallel: false);
                }
                else if (config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.GlobalizedBoundedNelderMead.ToString())
                {
                    optimizerMethod = new GlobalizedBoundedNelderMeadOptimizer(parameters, maxRestarts: config.Generations,
                        maxIterationsPrRestart: config.PopulationSize, seed: 42, maxDegreeOfParallelism: 1);
                }
                else if (config.Fitness.OptimizerTypeName == Enums.OptimizerTypeOptions.GridSearch.ToString())
                {
                    optimizerMethod = new GridSearchOptimizer(config.Genes.Select(s => new GridParameterSpec(RangeWithPrecision.Range(s.Min.Value, s.Max.Value, s.Precision.Value).ToArray())).ToArray(), runParallel: false);
                }
            }

            var result = await optimizerMethod.OptimizeBest(Minimize);

            return new IterationResult { ParameterSet = result.ParameterSet, Cost = IsMaximizing ? result.Error * -1 : result.Error };
        }

        public abstract Task<OptimizerResult> Minimize(double[] parameters);

    }
}
