using Microsoft.AspNetCore.Blazor.Hosting;
using System.Net.Http;
using Microsoft.JSInterop;

namespace Jtc.Optimization.BlazorClient
{
    public class Program
    {

        public static IJSRuntime JsRuntime { get; set; }
        public static HttpClient HttpClient { get; set; }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) => BlazorWebAssemblyHost.CreateDefaultBuilder().UseBlazorStartup<Startup>();
    }
}
