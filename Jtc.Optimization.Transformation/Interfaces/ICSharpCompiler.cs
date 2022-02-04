using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Jtc.Optimization.Transformation
{
    public interface ICSharpCompiler
    {
        Task<Assembly> CreateAssembly(string code);
        Task<Stream> GetStream(string code);
        Func<double[], Task<double>> GetDelegate(Assembly assembly);
    }
}