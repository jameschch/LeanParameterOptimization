﻿using Jtc.Optimization.LeanOptimizer.Base;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Jtc.Optimization.LeanOptimizer
{
    public class NestedCrossSharpeMaximizer : SharpeMaximizer
    {

        public override string Name { get; set; } = "NestedCrossSharpe";

        public NestedCrossSharpeMaximizer(IOptimizerConfiguration config, IFitnessFilter filter) : base(config, filter)
        {
        }
        //todo: configurable k-folds
        public override Dictionary<string, decimal> GetScore(Dictionary<string, object> list, IOptimizerConfiguration config)
        {
            //todo: threads
            var score = base.GetScore(list, config);

            var period = Config.StartDate.Value - Config.EndDate.Value.Date.AddDays(1).AddMilliseconds(-1);
            var oneThird = period.Ticks / 3;

            var oneThirdConfig = ((OptimizerConfiguration)Config).Clone();
            oneThirdConfig.EndDate = Config.StartDate.Value.AddTicks(oneThird);
            //todo: single app domain
            MultipleContextIsolator.Instance.RunAlgorithm(list, oneThirdConfig).Select(s => score[s.Key] += s.Value);

            var twoThirdsConfig = ((OptimizerConfiguration)Config).Clone();
            twoThirdsConfig.EndDate = Config.StartDate.Value.AddTicks(oneThird * 2);
            //todo: single app domain
            MultipleContextIsolator.Instance.RunAlgorithm(list, oneThirdConfig).Select(s => score[s.Key] += s.Value);
            return score.ToDictionary(d => d.Key, d => d.Value / 3);

        }

    }
}
