using Blazor.DynamicJavascriptRuntime.Evaluator;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient.Shared
{
    public class SidebarBase : ComponentBase
    {

        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public BlazorClientState BlazorClientState { get; set; }
        protected string Active { get; set; }
        protected string ConfigSaved { get { return GetConfigSaved(); } }
        protected string ChartDataSaved { get { return GetChartDataSaved(); } }

        protected async override Task OnInitializedAsync()
        {
            BlazorClientState.SubscribeStateHasChanged(typeof(SidebarBase), () => this.StateHasChanged());
            await base.OnInitializedAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("Sidebar.initialize");

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

        private string GetConfigSaved()
        {
            using (dynamic context = new EvalContext(JSRuntime))
            {
                (context as EvalContext).Expression = () => context.MainInterop.fetchConfig();
                var json = (context as EvalContext).Invoke<string>();

                return string.IsNullOrEmpty(json) ? "d-none" : null;
            }
        }

        private string GetChartDataSaved()
        {
            using (dynamic context = new EvalContext(JSRuntime))
            {
                (context as EvalContext).Expression = () => context.MainInterop.fetchChartData();
                var json = (context as EvalContext).Invoke<string>();

                return string.IsNullOrEmpty(json) ? "d-none" : null;
            }
        }

    }
}
