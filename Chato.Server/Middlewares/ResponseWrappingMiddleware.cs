using Chatto.Shared;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;


namespace Chato.Server.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class ResponseWrappingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseWrappingMiddleware> _logger;

        public ResponseWrappingMiddleware(RequestDelegate next, ILogger<ResponseWrappingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/chat/negotiate", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var originalBodyStream = context.Response.Body;
            using (var newBodyStream = new MemoryStream())
            {
                context.Response.Body = newBodyStream;

                await _next(context);

                if (context.Response.StatusCode >= 400)
                {
                    // Skip wrapping the response since it has been handled by the GlobalExceptionHandler
                    newBodyStream.Seek(0, SeekOrigin.Begin);
                    await newBodyStream.CopyToAsync(originalBodyStream);
                    return;
                }

                context.Response.Body = originalBodyStream;

                newBodyStream.Seek(0, SeekOrigin.Begin);
                var newBodyText = await new StreamReader(newBodyStream).ReadToEndAsync();

                var responseObject = newBodyText.Length > 0
                    ? JsonSerializer.Deserialize<object>(newBodyText)
                    : null;

                var wrappedResponse = new ResponseWrapper<object>
                {
                    IsSucceeded = context.Response.StatusCode >= 200 && context.Response.StatusCode < 400,
                    Response = responseObject as object,
                    StatusCode = context.Response.StatusCode
                };

                var wrappedResponseJson = JsonSerializer.Serialize(wrappedResponse, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = false
                });

                context.Response.ContentType = "application/json";
                context.Response.ContentLength = wrappedResponseJson.Length;
                await context.Response.WriteAsync(wrappedResponseJson);
                //}
                //catch (Exception ex)
                //{
                //    _logger.LogError(ex, "An error occurred while wrapping the response.");
                //    context.Response.StatusCode = 500;
                //    var errorResponse = new ResponseWrapper<string>
                //    {
                //        IsSucceeded = false,
                //        Response = "An internal server error occurred.",
                //        StatusCode = 500
                //    };
                //    var errorResponseJson = JsonSerializer.Serialize(errorResponse);
                //    context.Response.ContentType = "application/json";
                //    context.Response.ContentLength = errorResponseJson.Length;
                //    await context.Response.WriteAsync(errorResponseJson);
                //}
            }
        }
    }

}
