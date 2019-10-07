using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.Objects
{
    public class StatisticsBinding
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

    }
}
