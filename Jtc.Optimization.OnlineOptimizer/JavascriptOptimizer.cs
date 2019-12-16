using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public override OptimizerResult Minimize(double[] parameters)
        {
            Console.WriteLine("before:" + _code);

            var regex = new Regex(@".*function\s+([\d\w]+)\s*\(([\w\d,\s]+)\)");
            var matches = regex.Matches(_code)[0].Groups;

            var appending = new StringBuilder(_code);
            appending.Append("\r\n");
            appending.Append(matches[1].Value);
            appending.Append("(");

            //todo: validate number of params
            var split = matches[2].Value.Split(',').Select(s => s.Trim());

            for (int i = 0; i < parameters.Count(); i++)
            {
                var item = parameters[i];
                appending.Append(item.ToString("N"));
                if (i < parameters.Count()-1)
                {
                    appending.Append(",");
                }
            }
            appending.Append(");");

            var formatted = appending.ToString();
            Console.WriteLine("after:" + formatted);

            var error = new EvalContext(_jSRuntime).Invoke<double>(formatted);

            ActivityLogger.Add("Error: " + error.ToString("N"), DateTime.Now);
            return new OptimizerResult(parameters, error);
        }
    }
}
