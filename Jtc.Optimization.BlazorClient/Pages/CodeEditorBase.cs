using Blazor.DynamicJavascriptRuntime.Evaluator;
using Blazored.Toast.Services;
using Jtc.Optimization.Objects;
using Jtc.Optimization.OnlineOptimizer;
using Microsoft.AspNetCore.Components;
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

        protected override Task OnInitializedAsync()
        {
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
            string code = "";
            using (dynamic context = new EvalContext(JsRuntime))
            {
                (context as EvalContext).Expression = () => context.ace.edit("editor").getValue();
                code = await (context as EvalContext).InvokeAsync<string>();
            }

            Console.WriteLine(code);

            var optimizer = new JavascriptOptimizer(JsRuntime, code);
            var config = new OptimizerConfiguration
            {
                Genes = new GeneConfiguration[]
                {
                    new  GeneConfiguration{ MinDecimal = 1, MaxDecimal = 3 },
                    new  GeneConfiguration{ MinDecimal = 1, MaxDecimal = 3 },
                    new  GeneConfiguration{ MinDecimal = 1, MaxDecimal = 3 }
                },
                Generations = 10,
                Fitness = new FitnessConfiguration
                {
                    OptimizerTypeName = "RandomSearch"
                }
            };
            var result = optimizer.Start(config);

            ToastService.ShowSuccess("Best Error:" + result.Error.ToString());
            ToastService.ShowSuccess("Best Parameters:" + string.Join(",", result.ParameterSet));

        }

    }
}
