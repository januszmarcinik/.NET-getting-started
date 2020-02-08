using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NETCore3.Configuration;
using NETCore3.Entities;
using NETCore3.Middleware;
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

            services.AddHealthChecks();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder
                .Register(factory =>
                {
                    var initTeamMembers = new[]
                    {
                        new TeamMember(Guid.NewGuid(), "John", Role.DotNet, 5),
                        new TeamMember(Guid.NewGuid(), "Franc", Role.DotNet, 6),
                        new TeamMember(Guid.NewGuid(), "Robert", Role.JavaScript, 2),
                        new TeamMember(Guid.NewGuid(), "Alex", Role.DevOps, 5)
                    };
                    return new TeamMembersService(initTeamMembers);
                })
                .As<ITeamMembersService>()
                .SingleInstance();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseSerilogRequestLogging();

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
