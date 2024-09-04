using Chatto.Shared;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;


namespace Chato.Server.Middlewares
{
    using Chato.Server.Controllers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class ResponseWrappingMiddleware222
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseWrappingMiddleware> _logger;

        public ResponseWrappingMiddleware222(RequestDelegate next, ILogger<ResponseWrappingMiddleware> logger)
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

            // Check if the response is a file (content type other than JSON)
            if (context.Request.Path.Value.Contains($"/{AuthController.DownloadUrl}"))
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
                    Body = responseObject as object,
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
            }
        }

        private bool IsJsonResponse(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) &&
                   contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }
    }


}
