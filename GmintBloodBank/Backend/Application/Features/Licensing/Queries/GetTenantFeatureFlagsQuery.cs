using Application.Common.CQRS;
using Application.DTOs.Licensing;

namespace Application.Features.Licensing.Queries;

public record GetTenantFeatureFlagsQuery(Guid? TenantId = null) : IQuery<IReadOnlyList<FeatureFlagDto>>;
