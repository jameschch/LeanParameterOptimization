using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Blazor.DynamicJavascriptRuntime.Evaluator;
using Jtc.Optimization.Objects;
using Microsoft.JSInterop;
using SharpLearning.Optimization;

namespace Jtc.Optimization.OnlineOptimizer
{

    public class JavascriptOptimizer : OptimizerBase
    {

        private readonly IJSRuntime _jSRuntime;
        private readonly IBlazorClientConfiguration _blazorClientConfiguration;
        private double? _cost;

        public JavascriptOptimizer(IJSRuntime jSRuntime, IBlazorClientConfiguration blazorClientConfiguration)
        {
            _jSRuntime = jSRuntime;
            _blazorClientConfiguration = blazorClientConfiguration;
        }

        public async override Task<OptimizerResult> Minimize(double[] parameters)
        {
            //Console.WriteLine("before:" + _code);

            var regex = new Regex(@".*function\s+([\d\w]+)\s*\(([\w\d,\s]+)\)");
            var matches = regex.Matches(Code)[0].Groups;

            var appending = new StringBuilder(Code);
            appending.Append("\r\n");
            appending.Append(matches[1].Value);
            appending.Append("(");

            //todo: validate number of params
            var split = matches[2].Value.Split(',').Select(s => s.Trim());

            for (int i = 0; i < parameters.Count(); i++)
            {
                var item = parameters[i];
                appending.Append(item.ToString("N15").TrimEnd('0'));
                if (i < parameters.Count() - 1)
                {
                    appending.Append(",");
                }
            }
            appending.Append(");");

            var formatted = appending.ToString();
            //Console.WriteLine("after:" + formatted);

            var settings = new EvalContextSettings();
            settings.SerializableTypes.Add(typeof(DotNetObjectReference<JavascriptOptimizer>));
            dynamic context = new EvalContext(_jSRuntime, settings);

            if (_blazorClientConfiguration.EnableJavascriptOptimizerWorker)
            {
                //todo: support dotnet callback
                //(context as EvalContext).Expression = () => context.WorkerInterop.setWorkerCallback(DotNetObjectReference.Create(this), nameof(this.SetResult));
                //await (context as EvalContext).InvokeAsync<dynamic>();
                //(context as EvalContext).Reset();
                //(context as EvalContext).Expression = () => context.WorkerInterop.runWorker(formatted);
                //await (context as EvalContext).InvokeAsync<dynamic>();

                await _jSRuntime.InvokeVoidAsync("WorkerInterop.setWorkerCallback", DotNetObjectReference.Create(this), nameof(this.SetResult));
                await _jSRuntime.InvokeVoidAsync("WorkerInterop.runWorker", formatted);

                while (_cost == null)
                {
                    await Task.Delay(10);
                };
            }
            else
            {
                _cost = await (context as EvalContext).InvokeAsync<double>(formatted);
            }

            await Task.Run(() =>
            {
                ActivityLogger.Add(Guid.NewGuid().ToString(), Keys, parameters, _cost.Value);
                //ActivityLogger.Add("Parameters:", parameters);
                //ActivityLogger.Add("Cost:", _cost.Value);
                //ActivityLogger.StateHasChanged();
            });

            return new OptimizerResult(parameters, _cost.Value);
        }

        [JSInvokable]
        public void SetResult(double cost)
        {
            _cost = cost;
        }

    }
}
