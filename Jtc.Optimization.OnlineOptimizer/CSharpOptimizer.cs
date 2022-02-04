using Jtc.Optimization.BlazorClient;
using Jtc.Optimization.BlazorClient.Objects;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Objects.Interfaces;
using Jtc.Optimization.Transformation;
using SharpLearning.Optimization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Jtc.Optimization.OnlineOptimizer
{
    public class CSharpOptimizer : OptimizerBase
    {
        private readonly ICSharpCompiler _cSharpCompiler;
        private Func<double[], Task<double>> _minimize;

        public CSharpOptimizer(IBlazorClientConfiguration blazorClientConfiguration, IServiceProvider serviceProvider)
        {
            if (blazorClientConfiguration.CompileCSharpInBrowser)
            {
                _cSharpCompiler = (CSharpCompiler)serviceProvider.GetService<CSharpCompiler>();
            }
            else
            {
                _cSharpCompiler = (CSharpRemoteCompiler)serviceProvider.GetService<CSharpRemoteCompiler>();
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
                _minimize = _cSharpCompiler.GetDelegate(assembly);
            }

            var cost = await _minimize(parameters);

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
