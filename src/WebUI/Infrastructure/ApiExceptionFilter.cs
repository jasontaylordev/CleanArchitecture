using CleanArchitecture.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebUI.Filters;

public class ApiExceptionFilter : IEndpointFilter
{
    private readonly IDictionary<Type, Func<Exception, IResult>> _exceptionHandlers;

    public ApiExceptionFilter()
    {
        // Register known exception types and handlers.
        _exceptionHandlers = new Dictionary<Type, Func<Exception, IResult>>
            {
                { typeof(ValidationException), HandleValidationException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
                { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            };
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (Exception ex)
        {
            var result = HandleException(ex);

            if (result is not null)
            {
                return result;
            }

            throw;
        }
    }

    private IResult? HandleException(Exception ex)
    {
        var type = ex.GetType();

        if (_exceptionHandlers.ContainsKey(type))
        {
            return _exceptionHandlers[type].Invoke(ex);
        }

        return null;
    }

    private IResult HandleValidationException(Exception ex)
    {
        var exception = (ValidationException)ex;

        var details = new ValidationProblemDetails(exception.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        return Results.BadRequest(details);
    }

    private IResult HandleNotFoundException(Exception ex)
    {
        var exception = (NotFoundException)ex;

        var details = new ProblemDetails()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The specified resource was not found.",
            Detail = exception.Message
        };

        return Results.NotFound(details);
    }

    private IResult HandleUnauthorizedAccessException(Exception ex)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
        };

        return Results.Json(
            details, 
            statusCode: StatusCodes.Status401Unauthorized);
    }

    private IResult HandleForbiddenAccessException(Exception ex)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
        };

        return Results.Json(
            details,
            statusCode: StatusCodes.Status403Forbidden);
    }
}
