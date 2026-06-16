using Application.Common.CQRS;
using Application.DTOs.Licensing;
using Application.Interfaces.Persistence;
using Domain.Entities.Licensing;

namespace Application.Features.Licensing.Queries;

public sealed class GetTenantFeatureFlagsQueryHandler : IQueryHandler<GetTenantFeatureFlagsQuery, IReadOnlyList<FeatureFlagDto>>
{
    private readonly IRepository<FeatureFlag> _featureFlagRepository;
    private readonly IRepository<TenantFeature> _tenantFeatureRepository;

    public GetTenantFeatureFlagsQueryHandler(
        IRepository<FeatureFlag> featureFlagRepository,
        IRepository<TenantFeature> tenantFeatureRepository)
    {
        _featureFlagRepository = featureFlagRepository;
        _tenantFeatureRepository = tenantFeatureRepository;
    }

    public async Task<IReadOnlyList<FeatureFlagDto>> HandleAsync(GetTenantFeatureFlagsQuery query, CancellationToken cancellationToken = default)
    {
        var flags = await _featureFlagRepository.GetAllAsync(cancellationToken);

        IReadOnlyList<TenantFeature>? tenantFeatures = null;
        if (query.TenantId.HasValue)
        {
            var all = await _tenantFeatureRepository.GetAllAsync(cancellationToken);
            tenantFeatures = all
                .Where(tf => tf.TenantId == query.TenantId.Value)
                .ToList();
        }

        return flags
            .Where(f => !f.IsDeleted)
            .Select(f =>
            {
                var tenantOverride = tenantFeatures?
                    .FirstOrDefault(tf => tf.FeatureFlagId == f.Id);

                return new FeatureFlagDto
                {
                    Id = f.Id,
                    Key = f.Key,
                    Description = f.Description,
                    IsEnabled = tenantOverride?.IsEnabled ?? f.IsEnabled,
                    Scope = f.Scope,
                };
            })
            .ToList();
    }
}
