using Blazored.Toast;
using BlazorWorker.Core;
using Jtc.Optimization.BlazorClient.Attributes;
using Jtc.Optimization.BlazorClient.Objects;
using Jtc.Optimization.OnlineOptimizer;
using Jtc.Optimization.Transformation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Jtc.Optimization.BlazorClient
{
    public class Program
    {

        public static IServiceProvider ServiceProvider { get; set; }

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddApiAuthorization();

            builder.Services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);
            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<BlazorClientState>();
            builder.Services.AddSingleton<IBlazorClientConfiguration, BlazorClientConfiguration>();
            builder.Services.AddSingleton<ExportedWebAssemblyJSRuntime, ExportedWebAssemblyJSRuntime>();
            builder.Services.AddSingleton<IServiceProvider>(builder.Services.BuildServiceProvider());
            //todo: local compile
            builder.Services.AddScoped<CSharpCompiler, CSharpCompiler>();
            builder.Services.AddTransient<CSharpRemoteCompiler, CSharpRemoteCompiler>();
            builder.Services.AddTransient<CSharpOptimizer, CSharpOptimizer>();
            builder.Services.AddTransient<CSharpThreadedOptimizer, CSharpThreadedOptimizer>();
            builder.Services.AddTransient<JavascriptOptimizer, JavascriptOptimizer>();
            builder.Services.AddScoped<IMscorlibProvider, MscorlibRemoteProvider>();
            builder.Services.AddSingleton<IWorkerFactory, WorkerFactory>();
            builder.Services.AddTransient<IPlotlyLineSplitter, PlotlyLineSplitter>();
            builder.Services.AddTransient<IPlotlyBinder, PlotlyThreadedBinder>();
            builder.Services.AddSingleton<IPlotlyLineSplitterBackgroundWrapper, PlotlyLineSplitterBackgroundWrapper>();
            builder.Services.AddScoped<JavascriptFunctionValidatorAttribute>();

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

            JsonSerializer.SetDefaultResolver(StandardResolver.ExcludeNullCamelCase);

            builder.Services.AddWorkerFactory();

            ServiceProvider = builder.Services.BuildServiceProvider();

//#if !DEBUG
//            builder.Services.Configure<IISServerOptions>(options =>
//            {
//                options.AutomaticAuthentication = false;
//            });
//#endif

            await builder.Build().RunAsync();
        }
    }
}
