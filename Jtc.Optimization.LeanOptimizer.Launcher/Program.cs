using Jtc.Optimization.LeanOptimizer;
using NLog;
using System;

namespace Jtc.Optimization.Launcher.Legacy
{

    public class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                LogManager.Configuration.RemoveRuleByName("trace");
                LogManager.Configuration.RemoveTarget("trace");

                new OptimizerLauncher().Launch(args);               
            }
            catch (Exception ex)
            {
                LogProvider.ErrorLogger.Error(ex);
                throw new Exception("Unhandled Exception", ex);
            }
            LogManager.Shutdown();
            Console.ReadKey();

        }

    }
}
