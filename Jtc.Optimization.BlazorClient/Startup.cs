using Blazor.FileReader;
using Microsoft.AspNetCore.Builder;
using Blazored.Toast;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Transformation;
using System;
using Jtc.Optimization.OnlineOptimizer;
using Jtc.Optimization.Objects.Interfaces;

namespace Jtc.Optimization.BlazorClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);
            services.AddBlazoredToast();
            services.AddSingleton<BlazorClientState>();
            services.AddSingleton<IBlazorClientConfiguration, BlazorClientConfiguration>();
            services.AddSingleton<IServiceProvider>(services.BuildServiceProvider());
            //todo: local compile
            //services.AddSingleton<ICSharpCompiler, CSharpCompiler>();
            services.AddSingleton<CSharpRemoteCompiler, CSharpRemoteCompiler>();
            services.AddTransient<CSharpOptimizer, CSharpOptimizer>();
            services.AddTransient<JavascriptOptimizer, JavascriptOptimizer>();
            services.AddSingleton<IMscorlibProvider, MscorlibRemoteProvider>();

#if !DEBUG

            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
#endif
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }

    }
}
