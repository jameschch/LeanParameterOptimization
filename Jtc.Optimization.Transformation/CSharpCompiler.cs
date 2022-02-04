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
        private static string[] _objectMethods = new[] { "ToString", "Equals", "GetHashCode", "GetType" };

        public CSharpCompiler(HttpClient httpClient, IMscorlibProvider mscorlibProvider)
        {
            HttpClient = httpClient;
            _mscorlibProvider = mscorlibProvider;
        }

        public async virtual Task<Stream> GetStream(string code)
        {
            _mscorlib = _mscorlib ?? MetadataReference.CreateFromStream(new MemoryStream(await _mscorlibProvider.Get()));

            CSharpCompilation previousCompilation = null;

            var scriptCompilation = CSharpCompilation.CreateScriptCompilation(Guid.NewGuid().ToString(),
                CSharpSyntaxTree.ParseText(code, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script).WithLanguageVersion(LanguageVersion.Latest)),
                references: new[] { _mscorlib },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release, 
                concurrentBuild: false, usings: new[] { "System", "System.Threading.Tasks" }), previousCompilation);

            var errorDiagnostics = scriptCompilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error);
            if (errorDiagnostics.Any())
            {
                System.Diagnostics.Debug.WriteLine(string.Join(",\r\n", errorDiagnostics.Select(e => e.GetMessage())));
                return null;
            }

            var stream = new MemoryStream();
            if (scriptCompilation.Emit(stream).Success)
            {
                return await Task.FromResult(stream);
            }

            return null;

        }

        public async virtual Task<Assembly> CreateAssembly(string code)
        {
            var stream = await GetStream(code);
            stream.Position = 0;
            var loaded = Assembly.Load(new BinaryReader(stream).ReadBytes((int)stream.Length));
            return loaded;
        }

        public Func<double[], Task<double>> GetDelegate(Assembly assembly)
        {
            //var all = assembly.GetTypes().Select(s => s.FullName).ToList();
            var type = assembly.GetTypes().Single(s => s.DeclaringType != null && s.BaseType == typeof(object));
            var instance = Activator.CreateInstance(type);

            return (i) =>
            {
                var method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Single(n => !_objectMethods.Contains(n.Name));

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