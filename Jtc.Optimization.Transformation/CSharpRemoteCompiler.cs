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

        public override async Task<MemoryStream> CreateAssembly(string code)
        {
            return await CreateAssemblyRemotely(code);
        }

        public async Task<MemoryStream> CreateAssemblyRemotely(string code)
        {
            var response = await HttpClient.PostAsync("http://localhost:5000/api/compiler", new StringContent(code, Encoding.UTF8, "text/plain"));
            if (response.IsSuccessStatusCode)
            {
                var stream = new MemoryStream();
                await (await response.Content.ReadAsStreamAsync()).CopyToAsync(stream);
                return stream;
            }

            throw new Exception("Compile on server failed.");
        }

    }
}