using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace NETCore3.Configuration
{
    internal static class ApiInfoEndpoint
    {
        public static IApplicationBuilder UseApiInfoEndpoint(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            return app
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/api", async context =>
                    {
                        await context.Response.WriteAsync($"API is running on {env.EnvironmentName} environment");
                    });
                });
        }
    }
}