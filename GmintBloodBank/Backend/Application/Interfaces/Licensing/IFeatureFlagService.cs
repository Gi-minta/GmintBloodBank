namespace Application.Interfaces.Licensing;

/// <summary>
/// Servicio de Feature Flags. Determina si una funcionalidad
/// está habilitada de forma global o para un tenant específico.
/// </summary>
public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(string featureKey, Guid? tenantId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, bool>> GetAllFlagsAsync(Guid? tenantId = null, CancellationToken cancellationToken = default);
}
