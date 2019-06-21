using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization
{
    public static class LogShared
    {
        public static Logger Logger = LogManager.GetLogger("optimizer");
    }
}
