using Jtc.Optimization.Objects;
using Jtc.Optimization.Transformation;
using SharpLearning.Optimization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.OnlineOptimizer
{
    public class CSharpOptimizer : OptimizerBase
    {
        private readonly ICSharpCompiler _cSharpCompiler;
        private Func<double[], double> _minimize;

        public CSharpOptimizer(IBlazorClientConfiguration blazorClientConfiguration, IServiceProvider serviceProvider)
        {
            if (blazorClientConfiguration.CompileLocally)
            {
                _cSharpCompiler = (CSharpCompiler)serviceProvider.GetService(typeof(ICSharpCompiler));
            }
            else
            {
                _cSharpCompiler = (CSharpRemoteCompiler)serviceProvider.GetService(typeof(CSharpRemoteCompiler));
            }
        }

        public async override Task<OptimizerResult> Minimize(double[] parameters)
        {
            if (_minimize == null)
            {
                var assembly = await _cSharpCompiler.CreateAssembly(Code);
                _minimize = _cSharpCompiler.GetDelegate(assembly);
            }

            var cost = _minimize.Invoke(parameters);

            await Task.Run(() =>
            {
                ActivityLogger.Add("Parameters:", parameters);
                ActivityLogger.Add("Cost:", cost);
                //ActivityLogger.StateHasChanged();
            });

            return new OptimizerResult(parameters, cost);
        }
    }
}
