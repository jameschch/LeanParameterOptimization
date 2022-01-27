using QuantConnect;
using QuantConnect.Algorithm;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using System;

namespace Jtc.Optimization.LeanOptimizer.Example
{
    public class ParameterizedSharedAppDomainAlgorithm : QCAlgorithm
    {

        public ExponentialMovingAverage Fast;
        public ExponentialMovingAverage Slow;
        private decimal Take;

        public override void Initialize()
        {
            var startDate = DateTime.Parse(GetParameter("startDate") ?? "2001-01-01");
            var endDate = DateTime.Parse(GetParameter("endDate") ?? "1999-01-01"); //Invalid default will fail if used
            SetStartDate(startDate);
            SetEndDate(endDate);

            SetCash(100 * 1000);

            AddSecurity(SecurityType.Equity, "SPY");

            var fastPeriod = int.Parse(GetParameter("fast") ?? "10");
            var slowPeriod = int.Parse(GetParameter("slow") ?? "56");
            Take = decimal.Parse(GetParameter("take") ?? "0.1");

            Fast = EMA("SPY", fastPeriod);
            Slow = EMA("SPY", slowPeriod);
        }

        public void OnData(TradeBars data)
        {
            // wait for our indicators to ready
            if (!Fast.IsReady || !Slow.IsReady)
            {
                return;
            }

            if (Fast > Slow * 1.001m)
            {
                SetHoldings("SPY", 1);
            }
            else if (Portfolio["SPY"].HoldStock && Portfolio["SPY"].UnrealizedProfitPercent > Take)
            {
                Liquidate("SPY");
            }

        }
    }
}
