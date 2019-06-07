using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;
using Newtonsoft.Json;

namespace Optimization
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
            Name = "Sharpe";

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
                    output += string.Format("Start: {0}, End: {1}, ", Config.StartDate, Config.EndDate);
                }

                Dictionary<string,decimal> result = null;

                if (Config.UseSharedAppDomain)
                {
                    result = SingleAppDomainManager.RunAlgorithm(list, Config);
                }
                else
                {
                    result = OptimizerAppDomainManager.RunAlgorithm(list, Config);
                }

                if (result == null)
                {
                    return 0;
                }

                var fitness = CalculateFitness(result);

                output += string.Format("{0}: {1}", Name, fitness.Value);
                Program.Logger.Info(output);

                return fitness.Fitness;
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
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

        protected class FitnessResult
        {
            /// <summary>
            /// The value of the result
            /// </summary>
            public decimal Value { get; set; }
            /// <summary>
            /// The scaled or adjused fitness
            /// </summary>
            public double Fitness { get; set; }
        }


    }
}
