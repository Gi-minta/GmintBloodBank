using Application.Common.CQRS;
using Application.DTOs.Licensing;

namespace Application.Features.Licensing.Queries;

public record GetLicenseStatusQuery(Guid TenantId) : IQuery<LicenseDto>;
