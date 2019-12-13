using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazor.DynamicJavascriptRuntime.Evaluator;
using Microsoft.JSInterop;
using SharpLearning.Optimization;

namespace Jtc.Optimization.OnlineOptimizer
{


    public class JavascriptOptimizer : Optimizer
    {
        private readonly IJSRuntime _jSRuntime;
        private readonly string _code;

        public JavascriptOptimizer(IJSRuntime jSRuntime, string code)
        {
            _jSRuntime = jSRuntime;
            _code = code;
        }

        protected override OptimizerResult Minimize(double[] parameters)
        {
            var formatted = string.Format(_code, parameters.Select(s => (object)s).ToArray());
            Console.WriteLine(formatted);

            var error = new EvalContext(_jSRuntime).Invoke<double>(formatted);


            return new OptimizerResult(parameters, error);
        }
    }
}
