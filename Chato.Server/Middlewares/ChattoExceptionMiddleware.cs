
using Arkovean.Chat.Services.Validations;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

namespace Chato.Server.Middlewares;

public class ChattoExceptionMiddleware 
{
    private readonly RequestDelegate _next;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public ChattoExceptionMiddleware(RequestDelegate next, ProblemDetailsFactory problemDetailsFactory)
    {
        _next = next;
        _problemDetailsFactory = problemDetailsFactory;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ChatoException ex)
        {
            var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                httpContext,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation error",
                detail: "One or more validation errors occurred.",
                instance: httpContext.Request.Path
            );

            //var errors = new List<string>();
            //foreach (var error in ex.ChatoErrors)
            //{
            //    errors.Add($"{error.PropertyName} => {error.ErrorMessage}");
            //}
            //problemDetails.Extensions["errors"] = errors.ToArray();




            foreach (var item in ex.ChatoErrors)
            {
                problemDetails.Extensions.Add(item.PropertyName, item.ErrorMessage);
            }


            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var json = JsonSerializer.Serialize(problemDetails);
            await httpContext.Response.WriteAsync(json);
        }
    }

    //public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    //{
    //    try
    //    {
    //        await next(context);
    //    }
    //    catch (ChatoException chatoException)
    //    {

    //    }

    //    catch (Exception ex)
    //    {

    //    }
    //}
}
