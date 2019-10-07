using Jtc.Optimization.Objects;
using QuantConnect.Statistics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization
{
    public class StatisticsAdapter
    {

        public static decimal Translate(string key, Dictionary<string, decimal> list)
        {
            if (StatisticsBinding.Binding.ContainsKey(key))
            {
                return list[StatisticsBinding.Binding[key]];
            }

            return list[key];
        }

        public static Dictionary<string, decimal> Transform(AlgorithmPerformance performance, IDictionary<string, string> summary)
        {
            var list = performance.PortfolioStatistics.GetType().GetProperties().ToDictionary(k => k.Name, v => (decimal)v.GetValue(performance.PortfolioStatistics));
            list.Add("TotalNumberOfTrades", int.Parse(summary["Total Trades"]));
            list.Add("TotalFees", decimal.Parse(summary["Total Fees"].Substring(1)));

            return list;
        }

    }
}
