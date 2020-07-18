using BlazorWorker.Core;
using BlazorWorker.WorkerBackgroundService;
using Jtc.Optimization.BlazorClient.Objects;
using Jtc.Optimization.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jtc.Optimization.Transformation
{
    public interface IPlotlyLineSplitterBackgroundWrapper
    {
        Task<Dictionary<string, PlotlyData>> Split(string[] chunk);
    }
}