using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient.Pages
{
    public class WaitBase : ComponentBase
    {

        public string WaitClass { get; set; } = "";

        protected async override Task OnInitAsync()
        {
            WaitClass = "";
        }

        protected override Task OnAfterRenderAsync()
        {
            WaitClass = "displayNone";
            StateHasChanged();
            return Task.CompletedTask;
        }

    }
}
