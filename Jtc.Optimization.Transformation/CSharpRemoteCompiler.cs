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

    public class CSharpRemoteCompiler : CSharpCompiler
    {


        public CSharpRemoteCompiler(HttpClient httpClient, IMscorlibProvider mscorlibProvider) : base(httpClient, mscorlibProvider)
        {          
        }

        public async override Task<Assembly> CreateAssembly(string code)
        {
            return await CreateAssemblyRemotely(code);
        }

        public async Task<Assembly> CreateAssemblyRemotely(string code)
        {
            var response = await HttpClient.PostAsync("compile", new StringContent(code));
            if (response.IsSuccessStatusCode)
            {
                Assembly.Load(await response.Content.ReadAsByteArrayAsync());
            }

            throw new Exception("Compile on server failed.");
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