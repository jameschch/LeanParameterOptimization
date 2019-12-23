using Blazor.FileReader;
using Microsoft.AspNetCore.Builder;
using Blazored.Toast;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Jtc.Optimization.BlazorClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);
            services.AddBlazoredToast();
            services.AddSingleton<BlazorClientState>();
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
