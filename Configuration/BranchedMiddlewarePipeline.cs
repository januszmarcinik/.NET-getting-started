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
                    additionalMiddleware.UseExceptionHandlerMiddleware();
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
                appBuilder.Use((context, next) =>
                {
                    logger.LogWarning("Endpoint 'team-members' is not allowed while using branched middleware pipeline");
                    return next.Invoke();
                });
            });

            app.Run(async context =>
            {
                logger.LogError("Any branched route has not been mapped");
                await context.Response.WriteAsync("End of the request.");
            });

            return app;
        }
    }
}