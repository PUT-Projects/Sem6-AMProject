using ChatterAPI.Exceptions;

namespace ChatterAPI.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try {
            await next.Invoke(httpContext);
        }
        catch (BadRequestException ex) {
            await SendError(httpContext, ex, 404);
        }
        catch (ConflictException ex) {
            await SendError(httpContext, ex, 409);
        }
        catch (UnauthorizedException ex) {
            await SendError(httpContext, ex, 401);
        }
        catch (Exception ex) {
            _logger.LogError(ex, ex.Message);
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync("Something went wrong!");
        }
    }
    private async Task SendError(HttpContext httpContext, Exception ex, int statusCode)
    {
        _logger.LogWarning(ex.Message);
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsync(ex.Message);
    }
}
