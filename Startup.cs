using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NET.GettingStarted.HealthChecks;
using NET.GettingStarted.Services;
using NET.GettingStarted.Configuration;
using NET.GettingStarted.Middleware;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace NET.GettingStarted
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

            builder
                .RegisterType<CorrelationIdProvider>()
                .InstancePerLifetimeScope();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestDecorationMiddleware();
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseSerilogRequestLogging();

            app.UseMainMiddlewarePipeline();
            //app.UseBranchedMiddlewarePipeline();

            app.UseRouting()
                .UseApiInfoEndpoint(env)
                .UseTeamMembersEndpoints()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/health");
                    //endpoints.MapControllers();
                });
        }
    }
}
