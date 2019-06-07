using GeneticSharp.Domain.Chromosomes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization
{
    public class DualPeriodSharpeFitness : OptimizerFitness
    {

        public override string Name { get; set; } = "DualPeriodSharpe";

        public DualPeriodSharpeFitness(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
        {
        }

        public override double Evaluate(IChromosome chromosome)
        {
            var dualConfig = Clone<OptimizerConfiguration>((OptimizerConfiguration)Config);
            var start = Config.StartDate.Value;
            var end = Config.EndDate.Value;
            var diff = end - start;

            dualConfig.StartDate = end;
            dualConfig.EndDate = end + diff;

            var dualFitness = new OptimizerFitness(dualConfig, this.Filter);

            var first = base.Evaluate(chromosome);
            double second = -10;
            if (first > -10)
            {
                second = dualFitness.Evaluate(chromosome);
            }

            var fitness = new FitnessResult
            {
                Fitness = (first + second) / 2
            };
            fitness.Value = (decimal)base.GetValueFromFitness(fitness.Fitness);

            var output = string.Format($"Start: {Config.StartDate}, End: {Config.EndDate}, Start: {dualConfig.StartDate}, End: {dualConfig.EndDate}, "
            + $"Id: {((Chromosome)chromosome).Id}, Dual Period {this.Name}: {fitness.Value}");
            Program.Logger.Info(output);

            Config.StartDate = start;
            Config.EndDate = end;

            return fitness.Fitness;
        }

    }
}
