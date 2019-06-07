using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization
{
    public class ConfiguredFitness : OptimizerFitness
    {

        public ConfiguredFitness(IOptimizerConfiguration config) : base(config, null)
        {
            if (config.Fitness == null)
            {
                throw new ArgumentException("No fitness configuration was found.");
            }
            if (!config.Fitness.Scale.HasValue)
            {
                config.Fitness.Scale = 1;
            }
            if (!config.Fitness.Modifier.HasValue)
            {
                config.Fitness.Modifier = 1;
            }
            this.Name = Config.Fitness.Name;
        }

        //Fitness based on config settings
        protected override FitnessResult CalculateFitness(Dictionary<string, decimal> result)
        {
            var fitness = new FitnessResult();

            var raw = StatisticsAdapter.Translate(Config.Fitness.ResultKey, result);

            fitness.Value = raw;

            fitness.Fitness = (double)raw * Config.Fitness.Scale.Value * Config.Fitness.Modifier.Value;

            return fitness;
        }

        public override double GetValueFromFitness(double? fitness)
        {
            return (fitness.Value / Config.Fitness.Scale.Value) / Config.Fitness.Modifier.Value;
        }

    }
}
