using AuthService.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.API.Middleware;

public sealed class ActiveUserMiddleware
{
    private readonly RequestDelegate _next;

    public ActiveUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userIdValue = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? context.User.FindFirst("sub")?.Value
                ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid or missing user id claim");
                return;
            }

            var user = await userRepository.GetByIdAsync(userId);

            if (user is not null && !user.IsActive)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("User is inactive");
                return;
            }
        }

        await _next(context);
    }
}
