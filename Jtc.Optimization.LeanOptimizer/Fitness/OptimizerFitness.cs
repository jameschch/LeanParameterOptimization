using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using Jtc.Optimization.LeanOptimizer.Legacy;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Jtc.Optimization.LeanOptimizer
{


    /// <summary>
    /// Default optimizer behaviour using Sharpe ratio.
    /// </summary>
    /// <remarks>Default behaviour will nullify fitness for negative return</remarks>
    public class OptimizerFitness : IFitness
    {

        public virtual string Name { get; set; } = "Sharpe";
        public IOptimizerConfiguration Config { get; protected set; }
        public IFitnessFilter Filter { get; set; }
        protected double Scale { get; set; } = 0.02;
        protected const decimal ErrorRatio = -10;

        public OptimizerFitness(IOptimizerConfiguration config, IFitnessFilter filter)
        {
            Config = config;
            Filter = filter;
        }

        public virtual double Evaluate(IChromosome chromosome)
        {

            try
            {
                string output = "";
                var list = ((Chromosome)chromosome).ToDictionary();

                list.Add("Id", ((Chromosome)chromosome).Id);

                foreach (var item in list)
                {
                    output += item.Key + ": " + item.Value.ToString() + ", ";
                }

                if (Config.StartDate.HasValue && Config.EndDate.HasValue)
                {
                    output += string.Format("Start: {0}, End: {1}, ", Config.StartDate.Value.ToString("yyyy-MM-DD"), Config.EndDate.Value.ToString("yyyy-MM-DD"));
                }

                Dictionary<string, decimal> result = null;

                if (Config.UseSharedAppDomain)
                {
                    result = SingleAppDomainManager.Instance.RunAlgorithm(list, Config);
                }
                else
                {
                    result = LegacyAppDomainManager.Instance.RunAlgorithm(list, Config);
                }

                if (result == null)
                {
                    return 0;
                }

                var fitness = CalculateFitness(result);

                output += string.Format("{0}: {1}", Name, fitness.Value);
                LogProvider.OptimizerLogger.Info(output);

                return fitness.Fitness;
            }
            catch (Exception ex)
            {
                LogProvider.ErrorLogger.Error(ex);
                return 0;
            }
        }

        protected virtual FitnessResult CalculateFitness(Dictionary<string, decimal> result)
        {
            var fitness = new FitnessResult();

            var ratio = result["SharpeRatio"];

            if (Filter != null && !Filter.IsSuccess(result, this))
            {
                ratio = ErrorRatio;
            }

            fitness.Value = ratio;

            fitness.Fitness = (double)(System.Math.Max(ratio, ErrorRatio) + 10) * Scale;

            return fitness;
        }

        public virtual double GetValueFromFitness(double? fitness)
        {
            return fitness.Value / Scale - 10;
        }

        protected static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }


    }
}
