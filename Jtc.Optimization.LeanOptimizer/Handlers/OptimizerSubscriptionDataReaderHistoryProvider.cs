using QuantConnect.Data;
using QuantConnect.Lean.Engine.HistoricalData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.LeanOptimizer.Handlers
{
    public class OptimizerSubscriptionDataReaderHistoryProvider : SubscriptionDataReaderHistoryProvider
    {

        private bool _initialized;
        private static object locker = new object();

        public override void Initialize(HistoryProviderInitializeParameters parameters)
        {
            lock (locker)
            {
                if (_initialized)
                {
                    return;
                }
                base.Initialize(parameters);
                _initialized = true;
            }
        }

    }
}
