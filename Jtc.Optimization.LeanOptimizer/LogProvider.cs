using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.LeanOptimizer
{
    public class LogProvider
    {

        public static Logger OptimizerLogger { get; set; } = LogManager.GetLogger("optimizer");
        public static Logger GenerationsLogger { get; set; } = LogManager.GetLogger("generations");
        public static Logger ErrorLogger { get; set; } = LogManager.GetLogger("error");
        public static Logger TraceLogger { get; set; } = LogManager.GetLogger("trace");

    }
}
