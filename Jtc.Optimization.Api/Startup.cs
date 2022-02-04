using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Jtc.Optimization.Transformation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Jtc.Optimization.Api
{
    public class Startup
    {
        private const string PolicyName = "AllowAnyOrigin";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(m => m.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            //services.AddCors(options => options.AddPolicy(PolicyName, builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            services.AddSingleton(Configuration);
            services.AddScoped<ICSharpCompiler, CSharpCompiler>();
            services.AddScoped<IMscorlibProvider, MscorlibProvider>();
            services.AddSingleton<HttpClient, HttpClient>();

            services.AddLogging(loggingBuilder =>
             {
                 // configure Logging with NLog
                 loggingBuilder.ClearProviders();
                 loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                 loggingBuilder.AddNLog("nlog.config");
             });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var ex = context.Features.Get<IExceptionHandlerPathFeature>().Error;
                var response = new { error = ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }));

            app.UseCors(builder => builder
               //.WithOrigins("http://optimizer.ml", "https://optimizer.ml", "http://www.optimizer.ml", "https://www.optimizer.ml", "http://localhost:61221")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowAnyOrigin()
               .SetIsOriginAllowedToAllowWildcardSubdomains()
               //.AllowCredentials()
               );
            //app.UseHttpsRedirection();
            app.UseMvc();


        }
    }
}
