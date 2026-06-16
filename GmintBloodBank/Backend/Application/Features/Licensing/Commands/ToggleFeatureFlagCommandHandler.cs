using Application.Common.CQRS;
using Application.DTOs.Licensing;
using Application.Interfaces.Persistence;
using Domain.Entities.Licensing;

namespace Application.Features.Licensing.Commands;

public sealed class ToggleFeatureFlagCommandHandler : ICommandHandler<ToggleFeatureFlagCommand, FeatureFlagDto>
{
    private readonly IRepository<FeatureFlag> _featureFlagRepository;
    private readonly IRepository<TenantFeature> _tenantFeatureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleFeatureFlagCommandHandler(
        IRepository<FeatureFlag> featureFlagRepository,
        IRepository<TenantFeature> tenantFeatureRepository,
        IUnitOfWork unitOfWork)
    {
        _featureFlagRepository = featureFlagRepository;
        _tenantFeatureRepository = tenantFeatureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FeatureFlagDto> HandleAsync(ToggleFeatureFlagCommand command, CancellationToken cancellationToken = default)
    {
        var featureFlag = await _featureFlagRepository.GetByIdAsync(command.FeatureFlagId, cancellationToken);
        if (featureFlag is null)
            throw new KeyNotFoundException($"FeatureFlag with Id {command.FeatureFlagId} not found.");

        if (command.TenantId is null)
        {
            // Toggle global flag
            featureFlag.IsEnabled = command.IsEnabled;
            _featureFlagRepository.Update(featureFlag);
        }
        else
        {
            // Toggle tenant-specific flag
            var allTenantFeatures = await _tenantFeatureRepository.GetAllAsync(cancellationToken);
            var tenantFeature = allTenantFeatures
                .FirstOrDefault(tf => tf.FeatureFlagId == command.FeatureFlagId && tf.TenantId == command.TenantId);

            if (tenantFeature is null)
            {
                tenantFeature = new TenantFeature
                {
                    TenantId = command.TenantId.Value,
                    FeatureFlagId = command.FeatureFlagId,
                    IsEnabled = command.IsEnabled,
                };
                await _tenantFeatureRepository.AddAsync(tenantFeature, cancellationToken);
            }
            else
            {
                tenantFeature.IsEnabled = command.IsEnabled;
                _tenantFeatureRepository.Update(tenantFeature);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new FeatureFlagDto
        {
            Id = featureFlag.Id,
            Key = featureFlag.Key,
            Description = featureFlag.Description,
            IsEnabled = command.TenantId is null
                ? featureFlag.IsEnabled
                : command.IsEnabled,
            Scope = featureFlag.Scope,
        };
    }
}
