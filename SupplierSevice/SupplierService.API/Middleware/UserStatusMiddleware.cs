using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace SupplierService.API.Middleware;

public sealed class UserStatusMiddleware
{
    private readonly RequestDelegate _next;

    public UserStatusMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        // Only check when request is authenticated.
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userIdValue = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? context.User.FindFirst("sub")?.Value
                ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdValue, out var userId))
            {
                var baseUrl = configuration["AuthService:BaseUrl"];

                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    var client = httpClientFactory.CreateClient(nameof(UserStatusMiddleware));
                    client.BaseAddress = new Uri(baseUrl);

                    var authHeader = context.Request.Headers.Authorization.ToString();
                    if (!string.IsNullOrWhiteSpace(authHeader))
                        client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authHeader);

                    HttpResponseMessage response;
                    try
                    {
                        response = await client.GetAsync($"api/users/{userId}/status", context.RequestAborted);
                    }
                    catch
                    {
                        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                        await context.Response.WriteAsync("Unable to validate user status");
                        return;
                    }

                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("User is inactive");
                        return;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorBody = await response.Content.ReadAsStringAsync(context.RequestAborted);

                        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                        await context.Response.WriteAsync(
                            $"Unable to validate user status. AuthService responded {(int)response.StatusCode} ({response.ReasonPhrase}). Body: {errorBody}");
                        return;
                    }

                    var payload = await response.Content.ReadFromJsonAsync<UserStatusDto>(cancellationToken: context.RequestAborted);

                    if (payload is null)
                    {
                        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                        await context.Response.WriteAsync("Unable to validate user status");
                        return;
                    }

                    if (!payload.IsActive)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("User is inactive");
                        return;
                    }
                }
            }
        }

        await _next(context);
    }

    private sealed class UserStatusDto
    {
        public bool IsActive { get; set; }
    }
}
