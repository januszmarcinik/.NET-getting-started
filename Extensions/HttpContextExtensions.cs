using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace NET.GettingStarted.Extensions
{
    internal static class HttpContextExtensions
    {
        public static Task WriteOk(this HttpContext context, object value) =>
            WriteWithStatus(context, value, HttpStatusCode.OK);

        public static Task WriteNotFound(this HttpContext context, object value) =>
            WriteWithStatus(context, value, HttpStatusCode.NotFound);

        private static Task WriteWithStatus(this HttpContext context, object value, HttpStatusCode statusCode)
        {
            context.Response.StatusCode = (int)statusCode;
            var result = value switch
            {
                string s => s,
                _ => JsonConvert.SerializeObject(value, Formatting.Indented)
            };
            return context.Response.WriteAsync(result);
        }

        public static async Task<T> GetObjectFromBody<T>(this HttpContext context)
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(body);
        }
    }
}
