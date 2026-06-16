using Application.Common.CQRS;
using Application.Interfaces.Persistence;
using Domain.Entities.BloodUnits;
using Domain.Entities.Inventory;

namespace Application.Features.BloodUnits.Commands;

public sealed class ReleaseBloodUnitCommandHandler : ICommandHandler<ReleaseBloodUnitCommand, bool>
{
    private readonly IRepository<BloodUnit> _bloodUnitRepository;
    private readonly IRepository<BloodUnitStatus> _statusRepository;
    private readonly IRepository<InventoryMovement> _movementRepository;
    private readonly IRepository<InventoryMovementType> _movementTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReleaseBloodUnitCommandHandler(
        IRepository<BloodUnit> bloodUnitRepository,
        IRepository<BloodUnitStatus> statusRepository,
        IRepository<InventoryMovement> movementRepository,
        IRepository<InventoryMovementType> movementTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _bloodUnitRepository = bloodUnitRepository;
        _statusRepository = statusRepository;
        _movementRepository = movementRepository;
        _movementTypeRepository = movementTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(ReleaseBloodUnitCommand command, CancellationToken cancellationToken = default)
    {
        var bloodUnit = await _bloodUnitRepository.GetByIdAsync(command.BloodUnitId, cancellationToken);
        if (bloodUnit is null)
            throw new KeyNotFoundException($"BloodUnit with Id {command.BloodUnitId} not found.");

        if (bloodUnit.IsReleased)
            return false;

        var statuses = await _statusRepository.GetAllAsync(cancellationToken);
        var releasedStatus = statuses.FirstOrDefault(s => s.Code == "TRANSFUSED");
        if (releasedStatus is not null)
            bloodUnit.StatusId = releasedStatus.Id;

        bloodUnit.IsReleased = true;
        _bloodUnitRepository.Update(bloodUnit);

        // Record the release movement
        var movementTypes = await _movementTypeRepository.GetAllAsync(cancellationToken);
        var releaseType = movementTypes.FirstOrDefault(mt => mt.Code == "RELEASE");
        if (releaseType is not null)
        {
            var movement = new InventoryMovement
            {
                BloodUnitId = command.BloodUnitId,
                MovementTypeId = releaseType.Id,
                MovementDate = DateTime.UtcNow,
            };
            await _movementRepository.AddAsync(movement, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
