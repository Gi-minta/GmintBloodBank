using Application.Common.CQRS;
using Application.DTOs.BloodUnits;

namespace Application.Features.BloodUnits.Commands;

public record RegisterScreeningCommand(
    Guid BloodUnitId,
    Guid TechnicianId,
    string HivResult,
    string HbsAgResult,
    string HcvResult,
    string VdrlResult,
    string ChagasResult,
    string? Notes
) : ICommand<BloodScreeningDto>;
