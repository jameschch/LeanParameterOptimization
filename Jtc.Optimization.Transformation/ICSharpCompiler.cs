using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Jtc.Optimization.Transformation
{
    public interface ICSharpCompiler
    {
        Task<MemoryStream> CreateAssembly(string code);
        Func<double[], Task<double>> GetDelegate(MemoryStream stream);
    }
}