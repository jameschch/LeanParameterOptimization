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

        protected HttpClient HttpClient { get; }
        private PortableExecutableReference _mscorlib;
        private readonly IMscorlibProvider _mscorlibProvider;

        public CSharpCompiler(HttpClient httpClient, IMscorlibProvider mscorlibProvider)
        {
            HttpClient = httpClient;
            _mscorlibProvider = mscorlibProvider;
        }

        public async virtual Task<MemoryStream> CreateAssembly(string code)
        {
            _mscorlib = _mscorlib ?? MetadataReference.CreateFromStream(new MemoryStream(await _mscorlibProvider.Get()));

            CSharpCompilation previousCompilation = null;

            var scriptCompilation = CSharpCompilation.CreateScriptCompilation(Guid.NewGuid().ToString(),
                CSharpSyntaxTree.ParseText(code, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script).WithLanguageVersion(LanguageVersion.Latest)),
                references: new[] { _mscorlib },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release, concurrentBuild: false, usings: new[] { "System", "System.Threading.Tasks" }), previousCompilation);

            var errorDiagnostics = scriptCompilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error);
            if (errorDiagnostics.Any())
            {
                Console.WriteLine(string.Join(",\r\n", errorDiagnostics.Select(e => e.GetMessage())));
                return null;
            }

            var stream = new MemoryStream();
            if (scriptCompilation.Emit(stream).Success)
            {
                return await Task.FromResult(stream);
            }

            return null;
        }

        public Func<double[], Task<double>> GetDelegate(MemoryStream stream)
        {
            var assembly = Assembly.Load(stream.ToArray());
            var type = assembly.GetTypes().Single(s => s.DeclaringType != null && s.BaseType == typeof(object));
            var instance = Activator.CreateInstance(type);

            var objectMethods = typeof(object).GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(o => o.Name);

            return (i) =>
            {
                var method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Single(n => !objectMethods.Contains(n.Name));

                var returnValue = method.Invoke(instance, new object[] { i });

                if (method.ReturnType != typeof(Task<double>))
                {
                    return Task.FromResult((double)returnValue);
                }

                return (Task<double>)returnValue;
            };
        }

    }
}