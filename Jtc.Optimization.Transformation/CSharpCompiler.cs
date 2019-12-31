using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Jtc.Optimization.Objects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Jtc.Optimization.Transformation
{

    public class CSharpCompiler : ICSharpCompiler
    {
        private readonly IBlazorClientConfiguration _blazorClientConfiguration;
        protected HttpClient HttpClient { get; }
        private PortableExecutableReference _mscorlib;

        public CSharpCompiler(HttpClient httpClient, IMscorlibProvider mscorlibProvider)
        {
            HttpClient = httpClient;
            _mscorlib = MetadataReference.CreateFromStream(new MemoryStream(mscorlibProvider.Get()));
        }

        public CSharpCompiler(IBlazorClientConfiguration blazorClientConfiguration, HttpClient httpClient, IMscorlibProvider mscorlibProvider)
        {
            //todo: load on demand through service
            //_mscorlib = ;
             _mscorlib = MetadataReference.CreateFromStream(new MemoryStream(mscorlibProvider.Get()));
            _blazorClientConfiguration = blazorClientConfiguration;
            HttpClient = httpClient;
        }

        public async virtual Task<Assembly> CreateAssembly(string code)
        {

            CSharpCompilation previousCompilation = null;

            var scriptCompilation = CSharpCompilation.CreateScriptCompilation(Guid.NewGuid().ToString(),
                CSharpSyntaxTree.ParseText(code, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script).WithLanguageVersion(LanguageVersion.Latest)),
                references: new[] { _mscorlib },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release, usings: new[] { "System" }), previousCompilation);

            var errorDiagnostics = scriptCompilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error);
            if (errorDiagnostics.Any())
            {
                Console.WriteLine(string.Join(",\r\n", errorDiagnostics.Select(e => e.GetMessage())));
                return null;
            }

            using (var peStream = new MemoryStream())
            {
                if (scriptCompilation.Emit(peStream).Success)
                {
                    return Assembly.Load(peStream.ToArray());
                }
            }

            return null;
        }

        public Func<double[], double> GetDelegate(Assembly assembly)
        {
            var type = assembly.GetTypes().Single(s => s.DeclaringType != null && s.BaseType == typeof(object));
            var instance = Activator.CreateInstance(type);

            var objectMethods = typeof(object).GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(o => o.Name);

            return (i) => (double)type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Single(n => !objectMethods.Contains(n.Name)).Invoke(instance, new object[] { i });
        }

    }
}