using DotsWebApi.Exceptions;

namespace DotsWebApi.Middleware;
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (GameNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            context.Response.StatusCode = 404;
            context.Response.ContentType = "application/json";
            var response = new { message = ex.Message };
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (InvalidMoveException ex)
        {
            _logger.LogWarning(ex.Message);
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            var response = new { message = ex.Message };
            await context.Response.WriteAsJsonAsync(response);
        }
        catch(InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message);
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            var response = new { message = ex.Message };
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            Console.WriteLine($"Error: {ex.Message}");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var response = new { message = "An unexpected error occurred. Please try again later." };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}