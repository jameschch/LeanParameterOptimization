using Jtc.Optimization.Objects.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Jtc.Optimization.BlazorClient;
using Jtc.Optimization.BlazorClient.Objects;
using System.Reflection;

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

        public override async Task<Assembly> CreateAssembly(string code)
        {
            return await CreateAssemblyRemotely(code);
        }

        public async Task<Assembly> CreateAssemblyRemotely(string code)
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
                var buffer = await response.Content.ReadAsByteArrayAsync();
                var loaded = Assembly.Load(buffer);
                return await Task.FromResult(loaded);
            }

            throw new Exception("Compile on server failed.");
        }

    }
}