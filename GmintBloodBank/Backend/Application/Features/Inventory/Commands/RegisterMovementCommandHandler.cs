using Application.Common.CQRS;
using Application.DTOs.Inventory;
using Application.Interfaces.Persistence;
using Domain.Entities.BloodUnits;
using Domain.Entities.Inventory;

namespace Application.Features.Inventory.Commands;

public sealed class RegisterMovementCommandHandler : ICommandHandler<RegisterMovementCommand, InventoryMovementDto>
{
    private readonly IRepository<InventoryMovement> _movementRepository;
    private readonly IRepository<BloodUnit> _bloodUnitRepository;
    private readonly IRepository<InventoryMovementType> _movementTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterMovementCommandHandler(
        IRepository<InventoryMovement> movementRepository,
        IRepository<BloodUnit> bloodUnitRepository,
        IRepository<InventoryMovementType> movementTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _movementRepository = movementRepository;
        _bloodUnitRepository = bloodUnitRepository;
        _movementTypeRepository = movementTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InventoryMovementDto> HandleAsync(RegisterMovementCommand command, CancellationToken cancellationToken = default)
    {
        var bloodUnit = await _bloodUnitRepository.GetByIdAsync(command.BloodUnitId, cancellationToken);
        if (bloodUnit is null)
            throw new KeyNotFoundException($"BloodUnit with Id {command.BloodUnitId} not found.");

        var movementType = await _movementTypeRepository.GetByIdAsync(command.MovementTypeId, cancellationToken);
        if (movementType is null)
            throw new KeyNotFoundException($"MovementType with Id {command.MovementTypeId} not found.");

        var movement = new InventoryMovement
        {
            BloodUnitId = command.BloodUnitId,
            MovementTypeId = command.MovementTypeId,
            PerformedById = command.PerformedById,
            FromLocationId = command.FromLocationId,
            ToLocationId = command.ToLocationId,
            Notes = command.Notes,
        };

        await _movementRepository.AddAsync(movement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new InventoryMovementDto
        {
            Id = movement.Id,
            BloodUnitId = movement.BloodUnitId,
            UnitCode = bloodUnit.UnitCode,
            MovementType = movementType.Code,
            MovementDate = movement.MovementDate,
            Notes = movement.Notes,
        };
    }
}
