using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SoftableDrive.Web.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private static readonly Dictionary<Type, Func<Exception, IActionResult>> ExceptionHandlers =
        new()
        {
            { typeof(ArgumentException), e => new BadRequestObjectResult(e.Message) },
            {
                typeof(InvalidOperationException),
                e => new ObjectResult(e.Message)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                }
            },
            { typeof(FileNotFoundException), e => new NotFoundObjectResult(e.Message) },
        };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        if (ExceptionHandlers.TryGetValue(exception.GetType(), out var handler))
        {
            var result = handler(exception);
            context.Response.StatusCode =
                (result as ObjectResult)?.StatusCode ?? (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(
                new { error = "An unexpected error occurred.", message = exception.Message }
            );
        }
    }
}
