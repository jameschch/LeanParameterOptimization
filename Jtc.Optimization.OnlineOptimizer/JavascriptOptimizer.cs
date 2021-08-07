using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Blazor.DynamicJavascriptRuntime.Evaluator;
using Jtc.Optimization.BlazorClient;
using Jtc.Optimization.BlazorClient.Objects;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using Microsoft.JSInterop;
using SharpLearning.Optimization;

namespace Jtc.Optimization.OnlineOptimizer
{

    public class JavascriptOptimizer : OptimizerBase
    {

        private readonly IJSRuntime _jSRuntime;
        private readonly IBlazorClientConfiguration _blazorClientConfiguration;
        private Dictionary<string, double?> _allResults;
        private double? _cost;

        public JavascriptOptimizer(IJSRuntime jSRuntime, IBlazorClientConfiguration blazorClientConfiguration)
        {
            _jSRuntime = jSRuntime;
            _blazorClientConfiguration = blazorClientConfiguration;
        }

        public async override Task<OptimizerResult> Minimize(double[] parameters)
        {
            if (CancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            //Console.WriteLine("before:" + _code);
            _allResults = new Dictionary<string, double?>();

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

            string key = null;

            if (_blazorClientConfiguration.EnableOptimizerWorker)
            {
                //todo: support dotnet callback
                //(context as EvalContext).Expression = () => context.WorkerInterop.setWorkerCallback(DotNetObjectReference.Create(this), nameof(this.SetResult));
                //await (context as EvalContext).InvokeAsync<dynamic>();
                //(context as EvalContext).Reset();
                //(context as EvalContext).Expression = () => context.WorkerInterop.runWorker(formatted);
                //await (context as EvalContext).InvokeAsync<dynamic>();


                if (_blazorClientConfiguration.EnableOptimizerMultithreading)
                {
                    key = string.Join(",", parameters);
                    if (_allResults.ContainsKey(key))
                    {
                        return new OptimizerResult(parameters, _allResults[key].Value);
                    }

                    await _jSRuntime.InvokeVoidAsync("WorkerPoolInterop.runWorker", DotNetObjectReference.Create(this), nameof(this.AddResult), formatted, key);

                    while (!_allResults.ContainsKey(key))
                    {
                        await Task.Delay(10);
                    };
                }
                else
                {
                    await _jSRuntime.InvokeVoidAsync("WorkerInterop.setWorkerCallback", DotNetObjectReference.Create(this), nameof(this.SetResult));
                    await _jSRuntime.InvokeVoidAsync("WorkerInterop.runWorker", formatted);

                    while (_cost == null)
                    {
                        await Task.Delay(10);
                    };
                }
            }
            else
            {
                _cost = await (context as EvalContext).InvokeAsync<double>(formatted);
            }

            await Task.Run(() =>
            {
                ActivityLogger.Add(Guid.NewGuid().ToString(), Keys, parameters, _cost ?? _allResults[key].Value);
                //ActivityLogger.Add("Parameters:", parameters);
                //ActivityLogger.Add("Cost:", _cost.Value);
                //ActivityLogger.StateHasChanged();
            });

            return new OptimizerResult(parameters, _cost ?? _allResults[key].Value);
        }

        [JSInvokable]
        public void SetResult(double cost)
        {
            _cost = cost;
        }

        [JSInvokable]
        public void AddResult(string key, double cost)
        {
            _allResults.Add(key, cost);
        }

    }
}
