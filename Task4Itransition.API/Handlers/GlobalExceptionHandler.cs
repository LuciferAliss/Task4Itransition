using Task4Itransition.Application.Exceptions;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Task4Itransition.Application.Abstracts;

namespace Task4Itransition.API.Handlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, message) = GetExceptionDetails(exception);

        _logger.LogError(exception, message);

        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(message, cancellationToken);

        return true;
    }

    private (HttpStatusCode statusCode, string message) GetExceptionDetails(Exception exception)
    {
        if (exception is IHttpError httpError)
        {
            return (httpError.StatusCode, exception.Message);
        }

        return (HttpStatusCode.InternalServerError, $"An unexpected error occurred: {exception.Message}");
    }
}
