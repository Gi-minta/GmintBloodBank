using Application.Common.CQRS;

namespace Application.Features.BloodUnits.Commands;

public record ReleaseBloodUnitCommand(Guid BloodUnitId) : ICommand<bool>;
