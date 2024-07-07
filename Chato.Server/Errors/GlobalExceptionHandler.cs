using Arkovean.Chat.Services.Validations;
using Chatto.Shared;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
            if (exception is ChatoException chatoException)
            {
                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                        httpContext,
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Validation error",
                        detail: "One or more validation errors occurred.",
                        instance: httpContext.Request.Path
                    );

                foreach (var item in chatoException.ChatoErrors)
                {
                    problemDetails.Extensions.Add(item.PropertyName, item.ErrorMessage);
                }


                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                var response = new ResponseWrapper<ProblemDetails>
                {
                    Response = problemDetails,
                    IsSucceeded = false,
                    StatusCode = StatusCodes.Status400BadRequest
                };
                //var json = JsonSerializer.Serialize(problemDetails);
                await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            }
            else
            {
                var response = new ResponseWrapper<Exception>
                {
                    Response = exception,
                    IsSucceeded = false,
                    StatusCode = StatusCodes.Status400BadRequest
                };

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            }




            return true;
        }
    }
}