using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.Objects
{
    public class BlazorClientConfiguration : IBlazorClientConfiguration
    {

        public bool CompileLocally { get; set; } = false;
        public bool EnableOptimizerWorker { get; set; } = true;
        public bool EnableOptimizerMultithreading { get; set; } = false;

    }
}
