﻿using System.Collections.Generic;

namespace Jtc.Optimization.LeanOptimizer
{
    public class FitnessFilter : IFitnessFilter
    {

        /// <summary>
        /// Applies standard filters to eliminate some false positive optimizer results
        /// </summary>
        /// <param name="result">The statistic results</param>
        /// <param name="fitness">The calling fitness</param>
        /// <returns></returns>        
        public bool IsSuccess(Dictionary<string, decimal> result, OptimizerFitness fitness)
        {
            if (!fitness.Config.EnableFitnessFilter)
            {
                return true;
            }

            //using config ignore a result with negative return or disable this single filter and still apply others
            if (fitness.GetType() != typeof(CompoundingAnnualReturnFitness)
                && !fitness.Config.IncludeNegativeReturn && result["CompoundingAnnualReturn"] < 0)
            {
                return false;
            }

            //must meet minimum trading activity if configured
            if (fitness.Config.MinimumTrades > 0 && result["TotalNumberOfTrades"] < fitness.Config.MinimumTrades)
            {
                return false;
            }

            //Consider not trading a failure
            if (result["TotalNumberOfTrades"] == 0)
            {
                return false;
            }

            //Consider 100% loss rate, drawdown or net profit a failure
            if (result["LossRate"] >= 1 || result["Drawdown"] >= 1 || 
                (fitness.Config.ExcludeNetLoss && result["TotalNetProfit"] <= -1))
            {
                return false;
            }

            return true;
        }

    }

}
