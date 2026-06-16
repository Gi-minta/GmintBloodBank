using Application.Common.CQRS;
using Application.DTOs.Licensing;

namespace Application.Features.Licensing.Commands;

public record ToggleFeatureFlagCommand(
    Guid FeatureFlagId,
    bool IsEnabled,
    Guid? TenantId = null
) : ICommand<FeatureFlagDto>;
