/*
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using QuantConnect;
using QuantConnect.Algorithm;
using QuantConnect.Configuration;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using QuantConnect.Parameters;
using System;

namespace Jtc.Optimization.LeanOptimizer.Example
{
    public class ParameterizedSharedAppDomainAlgorithm : QCAlgorithm
    {


        private InstancedConfig _instancedConfig;
        private InstancedConfig InstancedConfig { get { return GetInstancedConfig(); } }

        public ExponentialMovingAverage Fast;
        public ExponentialMovingAverage Slow;
        private decimal Take;

        private InstancedConfig GetInstancedConfig()
        {
            _instancedConfig = _instancedConfig ?? new InstancedConfig(this);
            return _instancedConfig;
        }

        public override void Initialize()
        {
            //you must use instanced config with shared app domain
            SetStartDate(InstancedConfig.GetValue<DateTime>("startDate", new DateTime(2001, 1, 1)));
            SetEndDate(InstancedConfig.GetValue<DateTime>("endDate", new DateTime(1999, 1, 1))); //Invalid default will fail if used
            SetCash(100 * 1000);

            AddSecurity(SecurityType.Equity, "SPY");

            Fast = EMA("SPY", InstancedConfig.GetValue<int>("fast", 10));
            Slow = EMA("SPY", InstancedConfig.GetValue<int>("slow", 56));
            Take = InstancedConfig.GetValue<decimal>("take", 0.1m);
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
