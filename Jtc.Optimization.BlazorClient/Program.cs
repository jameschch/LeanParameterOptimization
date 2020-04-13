using System.Net.Http;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Jtc.Optimization.Objects.Interfaces;
using Jtc.Optimization.Objects;
using System;
using Jtc.Optimization.Transformation;
using Jtc.Optimization.OnlineOptimizer;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor.FileReader;
using Blazored.Toast;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Jtc.Optimization.BlazorClient
{
    public class Program
    {

        public static IJSRuntime JsRuntime { get; set; }
        public static HttpClient HttpClient { get; set; }

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);
            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<BlazorClientState>();
            builder.Services.AddSingleton<IBlazorClientConfiguration, BlazorClientConfiguration>();
            builder.Services.AddSingleton<IServiceProvider>(builder.Services.BuildServiceProvider());
            //todo: local compile
            //services.AddSingleton<ICSharpCompiler, CSharpCompiler>();
            builder.Services.AddSingleton<CSharpRemoteCompiler, CSharpRemoteCompiler>();
            builder.Services.AddTransient<CSharpOptimizer, CSharpOptimizer>();
            builder.Services.AddTransient<JavascriptOptimizer, JavascriptOptimizer>();
            builder.Services.AddSingleton<IMscorlibProvider, MscorlibRemoteProvider>();
            builder.Services.AddBaseAddressHttpClient();

            WebAssemblyHttpMessageHandlerOptions.DefaultCredentials = FetchCredentialsOption.Include;

#if !DEBUG

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
#endif

            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
