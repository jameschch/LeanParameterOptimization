using System;
using NLog;

namespace Optimization
{

    public class Program
    {

        public static Logger Logger = LogManager.GetLogger("optimizer");

        public static void Main(string[] args)
        {
            new OptimizerInitializer().Initialize(args);

            Console.ReadKey();
        }

    }
}