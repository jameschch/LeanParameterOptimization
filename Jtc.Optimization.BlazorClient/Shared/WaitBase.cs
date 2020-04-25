using Blazor.DynamicJavascriptRuntime.Evaluator;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient.Shared
{
    public class WaitBase : ComponentBase
    {

        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        //public int ProgressPercent { get; set; }
        public string Message { get; set; } = "Running...";
        [Inject]

        public BlazorClientState BlazorClientState { get; set; }
        protected string CancelClass { get; set; } = "d-none";

        public async Task Show()
        {
            using (dynamic context = new EvalContext(JsRuntime))
            {
                (context as EvalContext).Expression = () => context.jQuery("#wait").show();
                await (context as EvalContext).InvokeAsync<dynamic>();
            }
        }


        public async Task Hide()
        {
            using (dynamic context = new EvalContext(JsRuntime))
            {
                (context as EvalContext).Expression = () => context.jQuery("#wait").hide();
                await (context as EvalContext).InvokeAsync<dynamic>();
            }
        }

        public void ShowMessage(string message)
        {
            Message = message;
            StateHasChanged();
        }

        public void ShowConfirm()
        {
            if (CodeEditorBase.TokenSource == null) return;
            CancelClass = "cancel";
            StateHasChanged();
        }

        public void YesCancelClick()
        {
            CancelClass = "d-none";
            BlazorClientState.NotifyStateHasChanged(typeof(CodeEditorBase));
        }

        public void NoCancelClick()
        {
            CancelClass = "d-none";
            StateHasChanged();
        }

    }
}
