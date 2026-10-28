
using Microsoft.AspNetCore.Mvc;

namespace UserManagementAPI.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();

            var (status, title) = ex switch
            {
                ArgumentException =>
                    (400, "Invalid request"),

                InvalidOperationException
                    when ex.Message ==
                        "A user with this email already exists." =>
                    (409, "Resource conflict"),

                _ => (500, "Internal server error")
            };

            context.Response.StatusCode = status;
            context.Response.ContentType =
                "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == 500
                    ? "An unexpected error occurred."
                    : ex.Message,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(
                problem,
                cancellationToken: context.RequestAborted);
        }
    }
}
