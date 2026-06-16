using Application.Common.CQRS;
using Application.DTOs.BloodUnits;
using Application.Interfaces.Persistence;
using Domain.Entities.BloodUnits;

namespace Application.Features.BloodUnits.Commands;

public sealed class RegisterScreeningCommandHandler : ICommandHandler<RegisterScreeningCommand, BloodScreeningDto>
{
    private readonly IRepository<BloodScreening> _screeningRepository;
    private readonly IRepository<BloodUnit> _bloodUnitRepository;
    private readonly IRepository<BloodUnitStatus> _statusRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterScreeningCommandHandler(
        IRepository<BloodScreening> screeningRepository,
        IRepository<BloodUnit> bloodUnitRepository,
        IRepository<BloodUnitStatus> statusRepository,
        IUnitOfWork unitOfWork)
    {
        _screeningRepository = screeningRepository;
        _bloodUnitRepository = bloodUnitRepository;
        _statusRepository = statusRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BloodScreeningDto> HandleAsync(RegisterScreeningCommand command, CancellationToken cancellationToken = default)
    {
        var bloodUnit = await _bloodUnitRepository.GetByIdAsync(command.BloodUnitId, cancellationToken);
        if (bloodUnit is null)
            throw new KeyNotFoundException($"BloodUnit with Id {command.BloodUnitId} not found.");

        var allPositive = new[] { command.HivResult, command.HbsAgResult, command.HcvResult, command.VdrlResult, command.ChagasResult }
            .Any(r => r.Equals("POSITIVE", StringComparison.OrdinalIgnoreCase));

        var screening = new BloodScreening
        {
            BloodUnitId = command.BloodUnitId,
            TechnicianId = command.TechnicianId,
            HivResult = command.HivResult,
            HbsAgResult = command.HbsAgResult,
            HcvResult = command.HcvResult,
            VdrlResult = command.VdrlResult,
            ChagasResult = command.ChagasResult,
            IsApproved = !allPositive,
            RejectionReason = allPositive ? "One or more tests returned positive." : null,
            Notes = command.Notes,
        };

        await _screeningRepository.AddAsync(screening, cancellationToken);

        // Update blood unit status based on screening result
        var statuses = await _statusRepository.GetAllAsync(cancellationToken);
        var targetStatusCode = allPositive ? "DISCARDED" : "AVAILABLE";
        var targetStatus = statuses.FirstOrDefault(s => s.Code == targetStatusCode);
        if (targetStatus is not null)
        {
            bloodUnit.StatusId = targetStatus.Id;
            _bloodUnitRepository.Update(bloodUnit);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BloodScreeningDto
        {
            Id = screening.Id,
            BloodUnitId = screening.BloodUnitId,
            TechnicianName = screening.Technician?.FirstName,
            ScreeningDate = screening.ScreeningDate,
            HivResult = screening.HivResult,
            HbsAgResult = screening.HbsAgResult,
            HcvResult = screening.HcvResult,
            VdrlResult = screening.VdrlResult,
            ChagasResult = screening.ChagasResult,
            IsApproved = screening.IsApproved,
            Notes = screening.Notes,
        };
    }
}
