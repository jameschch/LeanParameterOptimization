using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization
{
    public class CompoundingAnnualReturnFitness : OptimizerFitness
    {

        public override string Name { get; set; } = "Return";

        public CompoundingAnnualReturnFitness(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
        {
        }

        double scale = 0.01;

        //Fitness based on Compounding Annual Return
        protected override FitnessResult CalculateFitness(Dictionary<string, decimal> result)
        {
            var fitness = new FitnessResult();

            var car = result["CompoundingAnnualReturn"];

            if (!Filter.IsSuccess(result, this))
            {
                car = -100m;
            }

            fitness.Value = car;

            fitness.Fitness = (double)car * scale;

            return fitness;
        }

        public override double GetValueFromFitness(double? fitness)
        {
            return fitness.Value / scale;
        }

    }
}
