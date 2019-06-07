using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using QuantConnect.Logging;

namespace Optimization
{
    public class LogSingleton
    {
        private static readonly Lazy<ILogHandler> instance = new Lazy<ILogHandler>(() =>
        {
            return new FileLogHandler("log.txt", true);
        });

        public static ILogHandler Instance { get { return instance.Value; } }

        private LogSingleton()
        {
        }
    }
}
