using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NETCore3.Configuration;
using NETCore3.HealthChecks;
using NETCore3.Services;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace NETCore3
{
    public class Startup
    {
        public ILifetimeScope AutofacContainer { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver());

            services.AddHealthChecks()
                .AddCheck<TeamMembersHealthCheck>("Team members health check");
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder
                .Register(_ => TeamMembersService.CreateDefault())
                .As<ITeamMembersService>()
                .SingleInstance();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseSerilogRequestLogging();

            //app.UseMainMiddlewarePipeline();
            app.UseBranchedMiddlewarePipeline();

            app.UseRouting()
                .UseApiInfoEndpoint(env)
                //.UseTeamMembersEndpoints()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/health");
                    endpoints.MapControllers();
                });
        }
    }
}
