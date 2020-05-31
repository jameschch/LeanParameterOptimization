using BlazorWorker.BackgroundServiceFactory;
using BlazorWorker.Core;
using BlazorWorker.WorkerBackgroundService;
using Jtc.Optimization.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.Transformation
{
    public class PlotlyLineSplitterBackgroundWrapper : IPlotlyLineSplitterBackgroundWrapper
    {
        private readonly IWorkerFactory _workerFactory;
        private IWorkerBackgroundService<PlotlyLineSplitter> _backgroundService;

        public PlotlyLineSplitterBackgroundWrapper(IWorkerFactory workerFactory)
        {
            _workerFactory = workerFactory;
        }

        public async Task<Dictionary<string, PlotlyData>> Split(string[] chunk)
        {
            if (_backgroundService == null)
            {
                var worker = await _workerFactory.CreateAsync();
                _backgroundService = await worker.CreateBackgroundServiceAsync<PlotlyLineSplitter>(new WorkerInitOptions
                {
                    DependentAssemblyFilenames = new[] { "Jtc.Optimization.Objects.dll", "Jtc.Optimization.Transformation.dll",
                    "System.Text.Json.dll" }
                });
            }

            return await _backgroundService.RunAsync(s => s.Split(chunk));
        }

    }
}
