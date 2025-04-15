namespace PingyThingy.Api.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add X-Content-Type-Options header
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // Add X-Frame-Options header
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        // Add Referrer-Policy header
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // Add Content-Security-Policy (Example: restrict to self, disable object-src)
        // Adjust this policy based on your specific needs. For a pure API, 'default-src 'none'' might be stricter.
        // context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; object-src 'none'; frame-ancestors 'none';");

        // Add Permissions-Policy (Example: disable common sensitive features)
        context.Response.Headers.Append(
            "Permissions-Policy",
            "camera=(), microphone=(), geolocation=(), payment=()"
        );

        await _next(context);
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
