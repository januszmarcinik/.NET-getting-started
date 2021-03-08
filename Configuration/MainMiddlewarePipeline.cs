using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NET.GettingStarted.Middleware;

namespace NET.GettingStarted.Configuration
{
    public static class MainMiddlewarePipeline
    {
        public static IApplicationBuilder UseMainMiddlewarePipeline(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetService<ILogger<Startup>>();
            
            app.UseExceptionHandlerMiddleware();
            
            app.Use(async (context, next) =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                logger.LogInformation("Start of request time measurement...");

                await next.Invoke();
                
                stopwatch.Stop();
                logger.LogInformation("Request finished in {Milliseconds} ms", stopwatch.ElapsedMilliseconds);
            });

            app.UseWhen(context => context.Request.Path.Value.Contains("team-members"), appBuilder =>
            {
                appBuilder.UseCorrelationIdMiddleware();
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Headers.TryGetValue("correlation-id", out var correlationId))
                {
                    logger.LogInformation("Request has been decorated with correlation ID {CorrelationId}", correlationId);
                }
                
                await next.Invoke();
            });

            return app;
        }
    }
}