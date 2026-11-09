using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TicketCanvas.Common.Http.Extensions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Detail = validationException.Message,
                Instance = httpContext.Request.Path,
                Extensions = new Dictionary<string, object?>{ ["errors"] = validationException.Errors }
            };

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        return false;
    }
}