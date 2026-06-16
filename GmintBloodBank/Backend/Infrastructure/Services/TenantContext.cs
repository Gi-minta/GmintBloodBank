using Application.Interfaces.Tenancy;

namespace Infrastructure.Services;

/// <summary>
/// Implementación de ITenantContext almacenada con duración de la solicitud HTTP
/// (registrada como Scoped en DependencyInjection).
/// </summary>
public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }

    public string? TenantCode { get; private set; }

    public bool HasTenant => TenantId.HasValue;

    public void SetTenant(Guid tenantId, string tenantCode)
    {
        TenantId   = tenantId;
        TenantCode = tenantCode;
    }
}
