using Arkovean.Chat.Services.Validations;
using Chatto.Shared;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections;
using System.Text;
using System.Text.Json;

namespace Chato.Server.Errors
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public GlobalExceptionHandler(ProblemDetailsFactory problemDetailsFactory)
        {
            _problemDetailsFactory = problemDetailsFactory;

        }


        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.ContentType = "application/json";

            var response = new ResponseWrapper<ProblemDetails>
            {
                IsSucceeded = false,
            };

            if (exception is ChatoException chatoException)
            {
                var problemDetails = GetProblemDetails(httpContext, "One or more validation errors occurred.");

                var sb = new StringBuilder();
                foreach (var item in chatoException.ChatoErrors)
                {
                    problemDetails.Extensions.Add(item.PropertyName, item.ErrorMessage);
                    sb.AppendLine($"{item.PropertyName} ==> {item.ErrorMessage}");
                }

                problemDetails.Extensions.Add("Reason", $"Validation error: {sb.ToString().TrimEnd()}");
                problemDetails.Status = null;

                response = new ResponseWrapper<ProblemDetails>
                {
                    Body = problemDetails,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            else
            {
                var problemDetails = GetProblemDetails(httpContext,  exception.Message);
                problemDetails.Status = null;

                
                foreach (DictionaryEntry dataItem in exception.Data)
                {
                    if (dataItem.Key is string key)
                    {
                        problemDetails.Extensions[key] = dataItem.Value?.ToString();
                    }
                }

                problemDetails.Extensions.Add("Reason", exception.Message);

                response = new ResponseWrapper<ProblemDetails>
                {
                    Body = problemDetails,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }


        private ProblemDetails GetProblemDetails(HttpContext httpContext, string detail)
        {
            return _problemDetailsFactory.CreateProblemDetails(httpContext, detail: detail, instance: httpContext.Request.Path);
        }
    }
}