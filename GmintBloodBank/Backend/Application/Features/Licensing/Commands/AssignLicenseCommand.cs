using Application.Common.CQRS;
using Application.DTOs.Licensing;

namespace Application.Features.Licensing.Commands;

public record AssignLicenseCommand(
    Guid TenantId,
    Guid LicensePlanId,
    string LicenseKey,
    DateTime StartDate,
    DateTime ExpirationDate
) : ICommand<LicenseDto>;
