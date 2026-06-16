using Application.Interfaces.Tenancy;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

/// <summary>
/// Resuelve el TenantId desde el claim "tenant_id" del JWT
/// o desde el header "X-Tenant-Id" como respaldo.
/// </summary>
public sealed class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Guid?> ResolveTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var context = _httpContextAccessor.HttpContext;

        if (context is null)
        {
            return Task.FromResult<Guid?>(null);
        }

        var claim = context.User.FindFirst("tenant_id")?.Value;

        if (Guid.TryParse(claim, out var tenantIdFromClaim))
        {
            return Task.FromResult<Guid?>(tenantIdFromClaim);
        }

        var header = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();

        if (Guid.TryParse(header, out var tenantIdFromHeader))
        {
            return Task.FromResult<Guid?>(tenantIdFromHeader);
        }

        return Task.FromResult<Guid?>(null);
    }
}
