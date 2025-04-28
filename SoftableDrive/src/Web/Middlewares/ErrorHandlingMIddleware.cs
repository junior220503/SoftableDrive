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
        };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context); //tenta realizar a operação
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message); //loga o erro
            await HandleExceptionAsync(context, ex); //chama o método que trata exceções
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            //não faz nada caso a response já esteja ocorrendo
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
