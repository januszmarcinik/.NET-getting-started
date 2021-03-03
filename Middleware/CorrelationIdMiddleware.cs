using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NETCore3.Services;

namespace NETCore3.Middleware
{
    internal class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CorrelationIdProvider _staticCorrelationIdMiddleware;
        private readonly ILogger _logger;

        public CorrelationIdMiddleware(
            RequestDelegate next, 
            CorrelationIdProvider staticCorrelationIdMiddleware, 
            ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _staticCorrelationIdMiddleware = staticCorrelationIdMiddleware;
            _logger = logger;
        }

        public Task InvokeAsync(HttpContext context, CorrelationIdProvider scopedCorrelationIdProvider)
        {
            _logger.LogInformation("Static correlation ID: {CorrelationId}", _staticCorrelationIdMiddleware.CorrelationId);
            context.Request.Headers.Add("correlation-id", $"{scopedCorrelationIdProvider.CorrelationId}");
            
            return _next.Invoke(context);
        }
    }
    
    internal static class CorrelationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder app) =>
            app.UseMiddleware<CorrelationIdMiddleware>();
    }
}
