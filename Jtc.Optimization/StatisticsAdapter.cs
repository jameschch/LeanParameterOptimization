using QuantConnect.Statistics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization
{
    public class StatisticsAdapter
    {
        public static Dictionary<string, string> Binding = new Dictionary<string, string>
        {
            {"Total Trades", "TotalNumberOfTrades"},
            {"Average Win","AverageWinRate"},
            {"Average Loss","AverageLossRate"},
            {"Compounding Annual Return ","CompoundingAnnualReturn"},
            {"Drawdown","Drawdown"},
            {"Expectancy","Expectancy"},
            {"Net Profit","TotalNetProfit"},
            {"Sharpe Ratio","SharpeRatio"},
            {"Loss Rate","LossRate"},
            {"Win Rate","WinRate"},
            {"Profit-Loss Ratio","ProfitLossRatio"},
            {"Alpha","Alpha"},
            {"Beta","Beta"},
            {"Annual Standard Deviation","AnnualStandardDeviation"},
            {"Annual Variance","AnnualVariance"},
            {"Information Ratio","InformationRatio"},
            {"Tracking Error","TrackingError"},
            {"Treynor Ratio","TreynorRatio"},
            {"Total Fees","TotalFees"}
        };

        public static decimal Translate(string key, Dictionary<string, decimal> list)
        {
            if (Binding.ContainsKey(key))
            {
                return list[Binding[key]];
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
