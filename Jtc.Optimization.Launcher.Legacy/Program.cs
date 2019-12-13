using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Jtc.Optimization.LeanOptimizer;

namespace Jtc.Optimization.Launcher.Legacy
{
    class Program
    {

        public static void Main(string[] args)
        {
            new OptimizerInitializer().Initialize(args);

            Console.ReadKey();
        }
    }
}
