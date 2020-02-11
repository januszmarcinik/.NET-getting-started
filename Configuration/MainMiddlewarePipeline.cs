using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NETCore3.Middleware;

namespace NETCore3.Configuration
{
    public static class MainMiddlewarePipeline
    {
        public static IApplicationBuilder UseMainMiddlewarePipeline(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetService<ILogger<Startup>>();
            
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            
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
                appBuilder.Use(async (context, next) =>
                {
                    context.Request.Headers.Add("correlation-id", $"{Guid.NewGuid()}");
                    await next.Invoke();
                });
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