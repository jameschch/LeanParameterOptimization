using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer
{
    public class WalkForwardPeriodCalculator
    {

        public Dictionary<int, DateTime[]> Calculate(IOptimizerConfiguration config)
        {
            var startDate = config.StartDate.Value.Date;
            var endDate = config.EndDate.Value.Date;

            return Calculate(config.Fitness.Folds, startDate, endDate);
        }

        private Dictionary<int, DateTime[]> Calculate(int folds, DateTime startDate, DateTime endDate)
        {
            var days = (int)Math.Ceiling((endDate - startDate).TotalDays) + 1;

            //even split days with truncating
            var lengths = Enumerable.Repeat(days / folds, folds).ToArray();
            //add extra day to last for remainder
            lengths[folds - 1] += (days % folds) > 0 ? 1 : 0;

            var inOutSample = new Dictionary<int, DateTime[]>();
            for (int i = 0; i < lengths.Length; i++)
            {
                inOutSample.Add(i, new[] {
                    startDate, //start of in sample
                    startDate.AddDays(lengths[i]-1), //end of in sample
                    startDate.AddDays(lengths[i]), //start of out sample
                    startDate.AddDays(lengths[i]).AddDays(lengths[i]/2) //end of out sample
                });

                //next in-sample start is offset half a fold
                startDate = inOutSample[i][0].AddDays(lengths[i] / 2);
            }
            inOutSample[lengths.Length - 1][3] = endDate;

            return inOutSample;
        }

    }
}
