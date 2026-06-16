namespace Application.Interfaces.Tenancy;

/// <summary>
/// Contexto del tenant (sede / banco de sangre) actual de la solicitud.
/// Implementado en Infrastructure a partir del JWT, header o subdominio.
/// </summary>
public interface ITenantContext
{
    Guid? TenantId { get; }

    string? TenantCode { get; }

    bool HasTenant { get; }

    void SetTenant(Guid tenantId, string tenantCode);
}
