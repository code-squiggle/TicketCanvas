using Microsoft.AspNetCore.Http;
using TicketCanvas.Common.Domain.Results;

namespace TicketCanvas.Common.Http.Extensions;

public static class ResultExtentions
{
    extension<T>(Result<T> result)
    {
        public IResult GetHttpResult()
        {
            if (result.IsSuccess)
                return TypedResults.Ok(result.Value);

            return result.ErrorType switch
            {
                ErrorType.NotFound => TypedResults.Problem(
                    detail: result.ErrorMessage,
                    instance: null,
                    statusCode: StatusCodes.Status404NotFound,
                    title: null,
                    type: null,
                    extensions: null
                ),

                ErrorType.Conflict => TypedResults.Problem(
                    detail: result.ErrorMessage,
                    instance: null,
                    statusCode: StatusCodes.Status409Conflict,
                    title: null,
                    type: null,
                    extensions: null
                ),

                ErrorType.None => throw new ApplicationException("Invalid ErrorType."),

                _ => throw new ApplicationException("Unexpected ErrorType.")
            };
        }

        public IResult GetHttpResult(string createdAtRouteName, object? createdAtRouteValues = null)
        {
            if (result.IsSuccess)
                return TypedResults.CreatedAtRoute(createdAtRouteName, createdAtRouteValues);

            return GetHttpResult(result);
        }
    }
}