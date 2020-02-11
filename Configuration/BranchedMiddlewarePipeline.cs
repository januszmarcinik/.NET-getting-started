using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NETCore3.Middleware;

namespace NETCore3.Configuration
{
    public static class BranchedMiddlewarePipeline
    {
        public static IApplicationBuilder UseBranchedMiddlewarePipeline(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetService<ILogger<Startup>>();
            
            app.Map("/api/role", appBuilder =>
            {
                appBuilder.UseWhen(context => context.Request.Path.Value.Contains("backend"), additionalMiddleware =>
                {
                    additionalMiddleware.UseMiddleware<ExceptionHandlerMiddleware>();
                });

                appBuilder
                    .UseRouting()
                    .UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
            });
            
            app.MapWhen(context => context.Request.Path.Value.Contains("team-members"), appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    logger.LogError("Endpoint 'team-members' is not allowed while using branched middleware pipeline.");
                    await context.Response.WriteAsync("End of the request.");
                });
            });

            app.Use((context, next) =>
            {
                logger.LogWarning("Any branched route has not been mapped.");
                return next.Invoke();
            });

            return app;
        }
    }
}