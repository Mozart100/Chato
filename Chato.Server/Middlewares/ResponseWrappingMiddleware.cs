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
    using System.Text.Json;
    using System.Text.Json.Serialization;

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
            // Skip middleware for SignalR negotiate and hub paths, or for specific download URLs
            if (context.Request.Path.StartsWithSegments("/chat/negotiate", StringComparison.OrdinalIgnoreCase) ||
                context.Request.Path.StartsWithSegments("/hub", StringComparison.OrdinalIgnoreCase) || // Assuming "/hub" is your SignalR hub path
                context.Request.Path.Value.Contains($"/{AuthController.DownloadUrl}"))
            {
                await _next(context);
                return;
            }

            var originalBodyStream = context.Response.Body;
            using (var newBodyStream = new MemoryStream())
            {
                context.Response.Body = newBodyStream;

                await _next(context);

                // Check if the response has already started (i.e., headers have been sent)
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response has already started, skipping wrapping.");
                    newBodyStream.Seek(0, SeekOrigin.Begin);
                    await newBodyStream.CopyToAsync(originalBodyStream);
                    return;
                }

                // Handle non-success responses (e.g., 4xx, 5xx)
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
                    Body = responseObject,
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
    }
}
