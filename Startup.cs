using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NETCore3.Configuration;
using NETCore3.Entities;
using NETCore3.Middleware;
using NETCore3.Services;
using Newtonsoft.Json.Serialization;

namespace NETCore3
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver());

            services.AddHealthChecks();

            services.AddSingleton<ITeamMembersService>(factory =>
            {
                var initTeamMembers = new []
                {
                    new TeamMember(Guid.NewGuid(), "John", Role.DotNet, 5),
                    new TeamMember(Guid.NewGuid(), "Franc", Role.DotNet, 6),
                    new TeamMember(Guid.NewGuid(), "Robert", Role.JavaScript, 2),
                    new TeamMember(Guid.NewGuid(), "Alex", Role.DevOps, 5)
                };
                return new TeamMembersService(initTeamMembers);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();

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
