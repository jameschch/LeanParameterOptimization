using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtc.Optimization.LeanOptimizer;
using NLog;

namespace Jtc.Optimization.Launcher.Legacy
{

    public class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                new OptimizerInitializer().Initialize(args);

            }
            catch (Exception ex)
            {
                LogProvider.ErrorLogger.Error(ex);
                throw new Exception("Unhandled Exception", ex);
            }
            Console.ReadKey();
        }

    }
}
