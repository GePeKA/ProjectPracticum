using Sprosi.Application.Common;

namespace Sprosi.Api.Middleware;

/// <summary>
/// Turns application exceptions into a JSON message and an HTTP status.
/// </summary>
public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Creates the middleware.
    /// </summary>
    /// <param name="next">Next request delegate.</param>
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the rest of the pipeline and maps known failures.
    /// </summary>
    /// <param name="context">Current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException exception)
        {
            context.Response.StatusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ForbiddenException => StatusCodes.Status403Forbidden,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest,
            };
            context.Response.ContentType = "application/json";
            var message = UserText.Get(context.Request.Headers.AcceptLanguage, exception.Code);
            await context.Response.WriteAsJsonAsync(new { code = exception.Code, message });
        }
    }
}
