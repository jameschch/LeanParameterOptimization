using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer
{
    public class NFoldCrossSharpeMaximizer : SharpeMaximizer
    {

        private int _folds = 2;
        public override string Name { get; set; } = "NFoldCrossSharpe";

        public NFoldCrossSharpeMaximizer(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
        {
            var folds = config.Fitness?.Folds ?? 2;
            if (folds > 0)
            {
                _folds = folds;
            }
            if (config.EndDate == null || config.StartDate == null)
            {
                throw new ArgumentException("Must supply start and end dates");
            }
        }

        public override Dictionary<string, decimal> GetScore(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            var firstConfig = ((OptimizerConfiguration)Config).Clone();

            firstConfig.EndDate = firstConfig.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            firstConfig.StartDate = firstConfig.StartDate.Value.Date;
            var period = (firstConfig.EndDate.Value - firstConfig.StartDate.Value).Ticks;
            var minimumPeriod = 863999999999;

            var foldSize = period / _folds;

            if (foldSize < minimumPeriod)
            {
                foldSize = minimumPeriod;
                _folds = 1;
            }

            firstConfig.EndDate = firstConfig.StartDate.Value.AddTicks(foldSize);

            var score = base.GetScore(list, firstConfig);
            //early stopping
            if (CalculateFitness(score).Value == ErrorRatio)
            {
                return score;
            }

            var previousConfig = firstConfig;

            for (int i = 0; i < _folds - 1; i++)
            {
                var iterationConfig = previousConfig.Clone();
                iterationConfig.StartDate = iterationConfig.EndDate.Value.AddTicks(1);
                iterationConfig.EndDate = iterationConfig.StartDate.Value.AddTicks(foldSize);

                var foldScore = base.GetScore(list, iterationConfig);
                //early stopping
                if (CalculateFitness(foldScore).Value == ErrorRatio)
                {
                    return foldScore;
                }

                score = foldScore.Select(s => new KeyValuePair<string, decimal>(s.Key, score[s.Key] + s.Value)).ToDictionary(k => k.Key, v => v.Value);

                previousConfig = iterationConfig;
            }

            var average = score.ToDictionary(d => d.Key, d => d.Value / _folds);

            return average;
        }

    }
}
