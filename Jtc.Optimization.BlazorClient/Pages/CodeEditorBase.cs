using Blazor.DynamicJavascriptRuntime.Evaluator;
using Blazored.Toast.Services;
using Jtc.Optimization.Objects;
using Jtc.Optimization.OnlineOptimizer;
using Jtc.Optimization.Transformation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient
{
    public class CodeEditorBase : ComponentBase
    {

        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        protected Models.MinimizeFunctionCode MinimizeFunctionCode { get; set; }
        [CascadingParameter]
        protected EditContext CurrentEditContext { get; set; }
        public string ActivityLog { get { return _activityLogger.Output; } }
        private ActivityLogger _activityLogger { get; set; } = new ActivityLogger();

        protected override Task OnInitializedAsync()
        {

            MinimizeFunctionCode = new Models.MinimizeFunctionCode
            {
                Code = "\r\nfunction Minimize(p1 /*p2, p3, etc*/)\r\n{\r\n\treturn;\r\n}"
            };

            using (dynamic context = new EvalContext(JsRuntime))
            {
                (context as EvalContext).Expression = () => context.ace.edit("editor").setTheme("ace/theme/monokai");
            }
            using (dynamic context = new EvalContext(JsRuntime))
            {
                (context as EvalContext).Expression = () => context.ace.edit("editor").session.setMode("ace/mode/javascript");
            }

            return base.OnInitializedAsync();
        }

        public async Task OptimizeClick()
        {

            Console.WriteLine(MinimizeFunctionCode.Code);

            var optimizer = new JavascriptOptimizer(JsRuntime, MinimizeFunctionCode.Code);
            var config = new OptimizerConfiguration
            {
                Genes = new GeneConfiguration[]
                {
                    new  GeneConfiguration{ MinDecimal = 0.001m, MaxDecimal = 3.0m, Precision = 6 },
                    new  GeneConfiguration{ MinDecimal = 0.001m, MaxDecimal = 3.0m, Precision = 6 }
                },
                Generations = 100,
                Fitness = new FitnessConfiguration
                {
                    OptimizerTypeName = "RandomSearch"
                }
            };

            try
            {
                var result = optimizer.Start(config, _activityLogger);
                ToastService.ShowSuccess("Best Error:" + result.Error.ToString("N"));
                ToastService.ShowSuccess("Best Parameters:" + string.Join(",", result.ParameterSet.Select(s => s.ToString("N"))));

            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
                throw;
            }
        }

    }
}
