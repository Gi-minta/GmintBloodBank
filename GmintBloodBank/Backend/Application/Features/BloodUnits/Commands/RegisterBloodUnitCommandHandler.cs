using Application.Common.CQRS;
using Application.DTOs.BloodUnits;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities.BloodUnits;
using Domain.Entities.Donors;

namespace Application.Features.BloodUnits.Commands;

public sealed class RegisterBloodUnitCommandHandler : ICommandHandler<RegisterBloodUnitCommand, BloodUnitDto>
{
    private readonly IRepository<BloodUnit> _bloodUnitRepository;
    private readonly IRepository<BloodComponent> _componentRepository;
    private readonly IRepository<BloodUnitStatus> _statusRepository;
    private readonly IRepository<BloodType> _bloodTypeRepository;
    private readonly IQrCodeService _qrCodeService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterBloodUnitCommandHandler(
        IRepository<BloodUnit> bloodUnitRepository,
        IRepository<BloodComponent> componentRepository,
        IRepository<BloodUnitStatus> statusRepository,
        IRepository<BloodType> bloodTypeRepository,
        IQrCodeService qrCodeService,
        IUnitOfWork unitOfWork)
    {
        _bloodUnitRepository = bloodUnitRepository;
        _componentRepository = componentRepository;
        _statusRepository = statusRepository;
        _bloodTypeRepository = bloodTypeRepository;
        _qrCodeService = qrCodeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<BloodUnitDto> HandleAsync(RegisterBloodUnitCommand command, CancellationToken cancellationToken = default)
    {
        var statuses = await _statusRepository.GetAllAsync(cancellationToken);
        var quarantineStatus = statuses.FirstOrDefault(s => s.Code == "QUARANTINE");
        if (quarantineStatus is null)
            throw new InvalidOperationException("Quarantine status not configured.");

        var component = await _componentRepository.GetByIdAsync(command.ComponentId, cancellationToken);
        if (component is null)
            throw new KeyNotFoundException($"Component with Id {command.ComponentId} not found.");

        var bloodType = await _bloodTypeRepository.GetByIdAsync(command.BloodTypeId, cancellationToken);
        if (bloodType is null)
            throw new KeyNotFoundException($"BloodType with Id {command.BloodTypeId} not found.");

        var unitCode = $"BU-{Guid.NewGuid():N}"[..16];
        var qrCode = _qrCodeService.GenerateQrCode(unitCode);

        var bloodUnit = new BloodUnit
        {
            UnitCode = unitCode,
            QrCode = qrCode,
            DonationId = command.DonationId,
            BloodTypeId = command.BloodTypeId,
            ComponentId = command.ComponentId,
            StatusId = quarantineStatus.Id,
            VolumeML = command.VolumeML,
            CollectionDate = command.CollectionDate,
            ExpirationDate = command.ExpirationDate,
            Notes = command.Notes,
        };

        await _bloodUnitRepository.AddAsync(bloodUnit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BloodUnitDto
        {
            Id = bloodUnit.Id,
            UnitCode = bloodUnit.UnitCode,
            QrCode = bloodUnit.QrCode,
            BloodType = bloodType.Code,
            Component = component.Name,
            Status = quarantineStatus.Code,
            VolumeML = bloodUnit.VolumeML,
            CollectionDate = bloodUnit.CollectionDate,
            ExpirationDate = bloodUnit.ExpirationDate,
            IsReleased = bloodUnit.IsReleased,
        };
    }
}
