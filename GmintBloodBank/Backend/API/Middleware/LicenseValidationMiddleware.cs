namespace API.Middleware;

public sealed class LicenseValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LicenseValidationMiddleware> _logger;

    public LicenseValidationMiddleware(RequestDelegate next, ILogger<LicenseValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // TODO: Validate license for the current tenant on each request
        await _next(context);
    }
}
