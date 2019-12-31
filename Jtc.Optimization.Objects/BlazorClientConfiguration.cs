using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.Objects
{
    public class BlazorClientConfiguration : IBlazorClientConfiguration
    {

        public bool CompileLocally { get; set; } = false;
        public bool EnableJavascriptOptimizerWorker { get; set; } = false;

    }
}
