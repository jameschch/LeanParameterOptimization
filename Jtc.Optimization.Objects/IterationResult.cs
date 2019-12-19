using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.Objects
{
    public class IterationResult
    {
        public double Cost { get; set; }
        public double[] ParameterSet { get; set; }
    }
}
