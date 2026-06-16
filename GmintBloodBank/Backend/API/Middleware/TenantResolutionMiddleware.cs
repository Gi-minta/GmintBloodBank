using Application.Interfaces.Tenancy;

namespace API.Middleware;

public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider, ITenantContext tenantContext)
    {
        var tenantId = await tenantProvider.ResolveTenantIdAsync();

        if (tenantId.HasValue)
        {
            tenantContext.SetTenant(tenantId.Value, string.Empty);
        }

        await _next(context);
    }
}
