using QuantConnect;
using QuantConnect.Algorithm;
using QuantConnect.Configuration;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using QuantConnect.Parameters;
using System;

namespace Jtc.Optimization.LeanOptimizer.Example
{
    public class ParameterizedWalkForwardAlgorithm : QCAlgorithm
    {

        public int FastPeriod = Config.GetInt("fast", 13);

        public int SlowPeriod = Config.GetInt("slow", 56);

        public ExponentialMovingAverage Fast;
        public ExponentialMovingAverage Slow;

        public override void Initialize()
        {
            SetStartDate(Config.GetValue<DateTime>("startDate", new DateTime(2015, 10, 07)));
            SetEndDate(Config.GetValue<DateTime>("endDate", new DateTime(2015, 10, 11)));
            SetCash(100 * 1000);

            AddSecurity(SecurityType.Equity, "SPY", Resolution.Daily);

            Fast = EMA("SPY", FastPeriod);
            Slow = EMA("SPY", SlowPeriod);

            SetWarmUp(SlowPeriod);
        }

        public void OnData(TradeBars data)
        {
            if (IsWarmingUp)
            {
                return;
            }

            // wait for our indicators to ready
            if (!Fast.IsReady || !Slow.IsReady) return;

            if (Fast > Slow * 1.001m)
            {
                SetHoldings("SPY", 1);
            }
            else if (Portfolio["SPY"].HoldStock && Portfolio["SPY"].UnrealizedProfitPercent > Config.GetValue<decimal>("take", 0.2m))
            {
                Liquidate("SPY");
            }

        }
    }
}
