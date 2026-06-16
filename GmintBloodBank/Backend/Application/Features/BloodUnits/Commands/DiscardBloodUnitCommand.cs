using Application.Common.CQRS;

namespace Application.Features.BloodUnits.Commands;

public record DiscardBloodUnitCommand(Guid BloodUnitId, string? Reason) : ICommand<bool>;
