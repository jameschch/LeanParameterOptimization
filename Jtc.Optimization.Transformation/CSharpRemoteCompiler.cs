using Jtc.Optimization.Objects.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

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
            //todo: data service
            var message = new HttpRequestMessage
            {
                Content = new StringContent(code, Encoding.UTF8, "text/plain"),
                RequestUri = new Uri(HttpClient.BaseAddress.Scheme + "://" + _blazorClientConfiguration.ApiUrl + "/api/compiler"),
                Method = HttpMethod.Post
            };
            message.SetBrowserRequestCredentials(BrowserRequestCredentials.Omit);
            message.SetBrowserRequestMode(BrowserRequestMode.Cors);

            var response = await HttpClient.SendAsync(message);

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