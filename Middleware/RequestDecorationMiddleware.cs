using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NETCore3.Middleware
{
    internal class RequestDecorationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestDecorationMiddleware(RequestDelegate next, ILogger<RequestDecorationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("--------------------------- B E G I N ---------------------------------------------------------------");
            _next(context);
            _logger.LogInformation("--------------------------- E N D -------------------------------------------------------------------");

            return Task.CompletedTask;
        }
    }

    internal static class RequestDecorationMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestDecorationMiddleware(this IApplicationBuilder app) =>
            app.UseMiddleware<RequestDecorationMiddleware>();
    }
}
