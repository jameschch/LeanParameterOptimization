using Jtc.Optimization.BlazorClient.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient.Models
{

    public class MinimizeFunctionCode
    {

        public string Language { get; set; } = "javascript";

        [JavascriptFunctionValidator]
        public string Code { get; set; }

    }
}
