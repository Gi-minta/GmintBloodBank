namespace Application.Interfaces.Licensing;

/// <summary>
/// Servicio de licenciamiento. Valida el estado de la licencia
/// del tenant actual (activa, expirada, suspendida, límites de uso).
/// </summary>
public interface ILicenseService
{
    Task<bool> IsLicenseActiveAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<bool> HasReachedUsageLimitAsync(Guid tenantId, string resourceKey, CancellationToken cancellationToken = default);
}
