using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Jtc.Optimization.Transformation
{
    public interface ICSharpCompiler
    {
        Task<Assembly> CreateAssembly(string code);
        Func<double[], double> GetDelegate(Assembly assembly);
    }
}