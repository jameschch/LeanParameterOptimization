using Blazor.DynamicJavascriptRuntime.Evaluator;
using Blazored.Toast.Services;
using Jtc.Optimization.BlazorClient.Shared;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using Jtc.Optimization.OnlineOptimizer;
using Jtc.Optimization.Transformation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;

namespace Jtc.Optimization.BlazorClient
{
    public class CodeEditorBase : ComponentBase
    {

        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public IServiceProvider ServiceProvider { get; set; }
        protected Models.MinimizeFunctionCode MinimizeFunctionCode { get; set; }
        [CascadingParameter]
        protected EditContext CurrentEditContext { get; set; }
        public string ActivityLog { get { return ActivityLogger?.Status; } }
        public ActivityLogger ActivityLogger { get; set; }
        protected WaitBase Wait { get; set; }
        Stopwatch _stopWatch = new Stopwatch();
        private Objects.OptimizerConfiguration _config;
        public static CancellationTokenSource TokenSource { get; set; }
        [Inject]
        public BlazorClientState BlazorClientState { get; set; }
        [Inject]
        public IBlazorClientConfiguration BlazorClientConfiguration { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ActivityLogger = new ActivityLogger(() => StateHasChanged(), m => Wait.ShowMessage(m));

            MinimizeFunctionCode = new Models.MinimizeFunctionCode
            {
                Code = Resource.JavascriptCodeSample
                // Code = "\r\nfunction minimize(p1 /*p2, anything, etc*/)\r\n{\r\n\treturn;\r\n}"
            };

            BlazorClientState.SubscribeStateHasChanged(typeof(CodeEditorBase), async () => await Cancel());

            await base.OnInitializedAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                using (dynamic context = new EvalContext(JSRuntime))
                {
                    (context as EvalContext).Expression = () => context.ace.edit("editor").setTheme("ace/theme/monokai");
                    await (context as EvalContext).InvokeAsync<dynamic>();

                    context.Reset();

                    (context as EvalContext).Expression = () => context.ace.edit("editor").session.setMode("ace/mode/javascript");
                    await (context as EvalContext).InvokeAsync<dynamic>();
                }
            }

            using (dynamic context = new EvalContext(JSRuntime))
            {
                (context as EvalContext).Expression = () => context.ace.edit("editor").session.setValue(MinimizeFunctionCode.Code);
                await (context as EvalContext).InvokeAsync<dynamic>();

                context.Reset();

                (context as EvalContext).Expression = () => context.ClientStorage.fetchConfig("config");
                var raw = await (context as EvalContext).InvokeAsync<string>();

                if (raw != null)
                {
                    _config = JsonSerializer.Deserialize<Objects.OptimizerConfiguration>(raw);
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected async Task OptimizeClick()
        {

            await Wait.Show();

            _stopWatch.Reset();
            _stopWatch.Start();

            IterationResult result = null;

            try
            {
                //Console.WriteLine(MinimizeFunctionCode.Code);

                OptimizerBase optimizer = null;
                if (MinimizeFunctionCode.Language == "javascript")
                {
                    optimizer = (JavascriptOptimizer)ServiceProvider.GetService(typeof(JavascriptOptimizer));
                }
                else if (MinimizeFunctionCode.Language == "csharp")
                {
                    optimizer = (CSharpThreadedOptimizer)ServiceProvider.GetService(typeof(CSharpThreadedOptimizer));
                }

                if (_config == null)
                {
                    ToastService.ShowError("No config was uploaded or created.");
                    await Wait.Hide();
                    return;
                }
                else
                {
                    var fitness = string.IsNullOrEmpty(_config.Fitness?.OptimizerTypeName) ? _config.FitnessTypeName : _config.Fitness.OptimizerTypeName;
                    ActivityLogger.ResetLog();
                    ActivityLogger.Add("Starting " + fitness);
                    optimizer.Initialize(MinimizeFunctionCode.Code, ActivityLogger);

                    TokenSource = new CancellationTokenSource();

                    var task = Task.Run(() => optimizer.Start(_config, TokenSource.Token), TokenSource.Token);

                    try
                    {
                        result = await task;

                    }
                    catch (TaskCanceledException)
                    {
                        CodeEditorBase.TokenSource = null;
                        await Wait.Hide();
                        ToastService.ShowInfo("Optimization was cancelled.");
                        TokenSource = null;
                        return;
                    }

                    TokenSource = null;

                    // Console.WriteLine(ActivityLogger.Log);

                    await JSRuntime.InvokeVoidAsync("ClientStorage.storeChartData", ActivityLogger.Log);
                    //todo: backticks
                    //dynamic context = new EvalContext(JSRuntime);
                    //(context as EvalContext).Expression = () => context.ClientStorage.storeChartData(ActivityLogger.Log);
                    //await (context as EvalContext).InvokeAsync<dynamic>();

                    ToastService.ShowSuccess("Chart data was saved.");
                }

            }
            catch (Exception ex)
            {
                await Wait.Hide();
                ToastService.ShowError(ex.Message);
                throw ex;
            }
            finally
            {
                await Wait.Hide();
            }

            _stopWatch.Stop();
            ToastService.ShowSuccess("Best Cost: " + result.Cost.ToString("N"));
            ToastService.ShowSuccess("Best Parameters: " + string.Join(",", result.ParameterSet.Select(s => s.ToString("N"))));
            ActivityLogger.Add("Best Cost: ", result.Cost);
            ActivityLogger.Add("Best Parameters: ", result.ParameterSet);
            ActivityLogger.Add("Total Time (s): ", _stopWatch.ElapsedMilliseconds / 1000);
        }

        public async Task LanguageChange(ChangeEventArgs e)
        {
            using (dynamic context = new EvalContext(JSRuntime))
            {
                (context as EvalContext).Expression = () => context.ace.edit("editor").session.setMode($"ace/mode/{e.Value}");
                await (context as EvalContext).InvokeAsync<dynamic>();
                (context as EvalContext).Reset();

                if (e.Value.ToString() == "javascript")
                {
                    MinimizeFunctionCode.Code = Resource.JavascriptCodeSample;
                }
                else if (e.Value.ToString() == "csharp")
                {
                    MinimizeFunctionCode.Code = Resource.CSharpCodeSample;
                }
                MinimizeFunctionCode.Language = e.Value.ToString();

                await (context as EvalContext).InvokeAsync<dynamic>($"ace.edit(\"editor\").session.setValue(`{MinimizeFunctionCode.Code}`)");
            }
            //StateHasChanged();
        }

        public async Task Cancel()
        {
            if (CodeEditorBase.TokenSource == null) return;

            CodeEditorBase.TokenSource.Cancel();
        }

    }
}
