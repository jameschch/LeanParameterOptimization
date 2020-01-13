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

        protected HttpClient HttpClient { get; }

        public MscorlibRemoteProvider(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<byte[]> Get()
        {
            var response = await HttpClient.GetAsync("http://localhost:5000/api/mscorlib");

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
