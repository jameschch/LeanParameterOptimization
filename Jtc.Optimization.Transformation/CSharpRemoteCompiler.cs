using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Jtc.Optimization.Objects.Interfaces;

namespace Jtc.Optimization.Transformation
{

    public class CSharpRemoteCompiler : CSharpCompiler
    {
        private readonly IBlazorClientConfiguration _blazorClientConfiguration;

        public CSharpRemoteCompiler(HttpClient httpClient, IMscorlibProvider mscorlibProvider, IBlazorClientConfiguration blazorClientConfiguration)
            : base(httpClient, mscorlibProvider)
        {
            _blazorClientConfiguration = blazorClientConfiguration;
        }

        public override async Task<MemoryStream> CreateAssembly(string code)
        {
            return await CreateAssemblyRemotely(code);
        }

        public async Task<MemoryStream> CreateAssemblyRemotely(string code)
        {
            var response = await HttpClient.PostAsync(_blazorClientConfiguration.ApiUrl + "/api/compiler", 
                new StringContent(code, Encoding.UTF8, "text/plain"));
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