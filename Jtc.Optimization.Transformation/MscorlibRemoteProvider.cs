using Jtc.Optimization.BlazorClient.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.Transformation
{
    public class MscorlibRemoteProvider : IMscorlibProvider
    {

        private readonly IBlazorClientConfiguration _blazorClientConfiguration;
        protected HttpClient HttpClient { get; }

        public MscorlibRemoteProvider(HttpClient httpClient, IBlazorClientConfiguration blazorClientConfiguration)
        {
            HttpClient = httpClient;
            _blazorClientConfiguration = blazorClientConfiguration;
        }

        public async Task<byte[]> Get()
        {
            var uri = new Uri(HttpClient.BaseAddress.Scheme + "://" + _blazorClientConfiguration.ApiUrl + "/api/mscorlib");
            var response = await HttpClient.GetAsync(uri);


            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
