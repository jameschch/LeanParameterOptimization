using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Jtc.Optimization.BlazorClient.Shared
{
    public class GeneConfigBase : ComponentBase
    {


        [Parameter]
        public Models.GeneConfiguration Gene { get; set; }


        protected string GetStep()
        {
            if (!Gene.Precision.HasValue)
            {
                return "1";
            }

            return Math.Pow(0.1, Gene.Precision.Value).ToString();
        }



    }
}
