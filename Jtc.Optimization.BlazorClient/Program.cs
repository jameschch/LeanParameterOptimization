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
using System.Collections.Generic;
using System.Linq;
using Utf8Json;
using Utf8Json.Resolvers;
using BlazorWorker.Core;
using Jtc.Optimization.BlazorClient.Objects;

namespace Jtc.Optimization.BlazorClient
{
    public class Program
    {

        //public static HttpClient HttpClient { get; set; }

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddApiAuthorization();

            builder.Services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);
            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<BlazorClientState>();
            builder.Services.AddSingleton<IBlazorClientConfiguration, BlazorClientConfiguration>();
            builder.Services.AddSingleton<IServiceProvider>(builder.Services.BuildServiceProvider());
            //todo: local compile
            //services.AddSingleton<ICSharpCompiler, CSharpCompiler>();
            builder.Services.AddSingleton<CSharpRemoteCompiler, CSharpRemoteCompiler>();
            builder.Services.AddTransient<CSharpOptimizer, CSharpOptimizer>();
            builder.Services.AddTransient<CSharpThreadedOptimizer, CSharpThreadedOptimizer>();
            builder.Services.AddTransient<JavascriptOptimizer, JavascriptOptimizer>();
            builder.Services.AddSingleton<IMscorlibProvider, MscorlibRemoteProvider>();
            builder.Services.AddSingleton<IWorkerFactory, WorkerFactory>();
            builder.Services.AddTransient<IPlotlyLineSplitter, PlotlyLineSplitter>();
            builder.Services.AddTransient<IPlotlyBinder, PlotlyThreadedBinder>();
            builder.Services.AddSingleton<IPlotlyLineSplitterBackgroundWrapper, PlotlyLineSplitterBackgroundWrapper>();

            builder.Services.AddTransient(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

            JsonSerializer.SetDefaultResolver(StandardResolver.ExcludeNullCamelCase);

            builder.Services.AddWorkerFactory();

#if !DEBUG
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
#endif

            await builder.Build().RunAsync();
        }
    }
}
