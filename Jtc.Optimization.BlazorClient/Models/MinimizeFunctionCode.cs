using Jtc.Optimization.BlazorClient.Attributes;
using Mono.WebAssembly.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jtc.Optimization.BlazorClient.Models
{

    public class MinimizeFunctionCode
    {

        [JavascriptFunctionValidator]
        public string Code { get; set; }

    }
}
