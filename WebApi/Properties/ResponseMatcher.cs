using Contracts.Errors.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Properties;

public static class ResponseMatcher
{
    public static IActionResult AsErrorResult<TResponse>(TResponse response) =>
        response switch
        {
            INotFoundError => GetResult<NotFoundResult, NotFoundObjectResult>(response),
            IValidationError => GetResult<BadRequestResult, BadRequestObjectResult>(response),
            IAlreadyExistsError => GetResult<ConflictResult, ConflictObjectResult>(response),
            IPermissionError => new ForbidResult(),
            IError => throw new ArgumentException(
                $"Unable to find an error handler for {response.GetType().Name}"),
            Unit => new NoContentResult(),
            _ => GetResult<OkResult, OkObjectResult>(response)
        };

    private static IActionResult GetResult<TCodeResult, TObjectResult>(object? data)
        where TCodeResult : StatusCodeResult
        where TObjectResult : ObjectResult
    {
        var msg = data;
        if (data is IError err)
            msg = err.Message is not null
                ? new
                {
                    Error = err.Message
                }
                : err.Message;

        var result = msg is null
            ? Activator.CreateInstance(typeof(TCodeResult)) as IActionResult
            : Activator.CreateInstance(typeof(TObjectResult), msg) as IActionResult;

        return result ?? new OkResult();
    }
}