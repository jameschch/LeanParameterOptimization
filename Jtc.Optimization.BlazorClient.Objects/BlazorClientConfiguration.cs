using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.BlazorClient.Objects
{
    public class BlazorClientConfiguration : IBlazorClientConfiguration
    {

        public bool CompileLocally { get; set; } = false;
        public bool EnableOptimizerWorker { get; set; } = true;
        public bool EnableOptimizerMultithreading { get; set; } = false;

#if DEBUG
        public string ApiUrl { get; set; } = "localhost:5000";
#else
        public string ApiUrl { get; set; } = "api.optimizer.ml";
#endif


    }
}
