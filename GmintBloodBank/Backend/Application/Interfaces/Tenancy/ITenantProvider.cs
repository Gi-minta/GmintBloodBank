namespace Application.Interfaces.Tenancy;

/// <summary>
/// Resuelve el tenant activo a partir de la solicitud HTTP
/// (claim del JWT, header X-Tenant-Id o subdominio).
/// </summary>
public interface ITenantProvider
{
    Task<Guid?> ResolveTenantIdAsync(CancellationToken cancellationToken = default);
}
