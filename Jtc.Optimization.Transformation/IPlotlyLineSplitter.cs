using Jtc.Optimization.Objects;
using System;
using System.Collections.Generic;

namespace Jtc.Optimization.Transformation
{
    public interface IPlotlyLineSplitter
    {
        Dictionary<string, PlotlyData> Split(string[] line);
    }
}