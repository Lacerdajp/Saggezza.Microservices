using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Client closed the request.
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            var statusCode = (int)HttpStatusCode.InternalServerError;

            var problem = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = statusCode,
                Type = "https://httpstatuses.com/500",
                Instance = context.Request.Path
            };

            if (_env.IsDevelopment())
            {
                problem.Detail = ex.Message;
                problem.Extensions["exception"] = ex.GetType().FullName;
                problem.Extensions["traceId"] = context.TraceIdentifier;
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions), context.RequestAborted);
        }
    }
}
