using BlazorWorker.BackgroundServiceFactory;
using BlazorWorker.Core;
using BlazorWorker.WorkerBackgroundService;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using Jtc.Optimization.Transformation;
using Microsoft.AspNetCore.Components;
using SharpLearning.Optimization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.OnlineOptimizer
{
    public class CSharpThreadedOptimizer : OptimizerBase
    {
        private readonly ICSharpCompiler _cSharpCompiler;
        private readonly IWorkerFactory _workerFactory;
        private IWorkerBackgroundService<MinimizeFacade> _backgroundService;
        private Func<double[], Task<double>> _minimize;

        public CSharpThreadedOptimizer(IBlazorClientConfiguration blazorClientConfiguration, IServiceProvider serviceProvider, 
            IWorkerFactory workerFactory)
        {
            if (blazorClientConfiguration.CompileLocally)
            {
                _cSharpCompiler = (CSharpCompiler)serviceProvider.GetService(typeof(ICSharpCompiler));
            }
            else
            {
                _cSharpCompiler = (CSharpRemoteCompiler)serviceProvider.GetService(typeof(CSharpRemoteCompiler));
            }

            _workerFactory = workerFactory;
        }

        public class MinimizeFacade
        {

            public async Task<double> Minimize(Func<double[], Task<double>> action, double[] parameters)
            {
                return await action.Invoke(parameters);
            }

        }

        public async override Task<OptimizerResult> Minimize(double[] parameters)
        {
            if (CancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            if (_minimize == null)
            {
                var assembly = await _cSharpCompiler.CreateAssembly(Code);

                var worker = await _workerFactory.CreateAsync();
                _backgroundService = await worker.CreateBackgroundServiceAsync<MinimizeFacade>();
                _minimize = _cSharpCompiler.GetDelegate(assembly);

            }

            var cost = await _backgroundService.RunAsync(r => r.Minimize(_minimize, parameters)).Result;

            await Task.Run(() =>
            {
                ActivityLogger.Add(Guid.NewGuid().ToString(), Keys, parameters, cost);
                //ActivityLogger.Add("Parameters:", parameters);
                //ActivityLogger.Add("Cost:", cost);
                //ActivityLogger.StateHasChanged();
            });

            return new OptimizerResult(parameters, cost);
        }
    }
}
