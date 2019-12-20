using Blazor.DynamicJavascriptRuntime.Evaluator;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient.Shared
{
    public class SidebarBase : ComponentBase
    {

        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        protected string Active { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("Sidebar.initialize");

                await base.OnAfterRenderAsync(firstRender);
            }
        }

        //protected Task DismissClick()
        //{
        //    Active = Active.Replace("active", "");

        //    return Task.CompletedTask;
        //}


        protected void NavigateClick(string page)
        {
            NavigationManager.NavigateTo($"/{page}");
        }

    }
}
