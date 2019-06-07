using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization
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
        }

        public override Dictionary<string, decimal> GetScore(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            var firstConfig = Clone((OptimizerConfiguration)Config);

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
                var iterationConfig = Clone((OptimizerConfiguration)previousConfig);
                iterationConfig.StartDate = iterationConfig.EndDate.Value.AddTicks(1);
                iterationConfig.EndDate = iterationConfig.StartDate.Value.AddTicks(foldSize);

                var foldScore = base.GetScore(list, iterationConfig);
                //early stopping
                if (CalculateFitness(foldScore).Value == ErrorRatio)
                {
                    return foldScore;
                }

                score = foldScore.ToDictionary(k => k.Key, v => score[v.Key] += v.Value);

                previousConfig = iterationConfig;
            }

            var average = score.ToDictionary(d => d.Key, d => d.Value / _folds);

            return average;
        }

    }
}
