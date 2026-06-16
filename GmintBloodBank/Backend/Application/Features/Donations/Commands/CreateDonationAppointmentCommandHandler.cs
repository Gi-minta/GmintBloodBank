using Application.Common.CQRS;
using Application.DTOs.Donations;
using Application.Interfaces.Persistence;
using Domain.Entities.Donations;
using Domain.Entities.Donors;

namespace Application.Features.Donations.Commands;

public sealed class CreateDonationAppointmentCommandHandler : ICommandHandler<CreateDonationAppointmentCommand, DonationAppointmentDto>
{
    private readonly IRepository<DonationAppointment> _appointmentRepository;
    private readonly IRepository<DonationStatus> _statusRepository;
    private readonly IRepository<Donor> _donorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDonationAppointmentCommandHandler(
        IRepository<DonationAppointment> appointmentRepository,
        IRepository<DonationStatus> statusRepository,
        IRepository<Donor> donorRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _statusRepository = statusRepository;
        _donorRepository = donorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DonationAppointmentDto> HandleAsync(CreateDonationAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        var donor = await _donorRepository.GetByIdAsync(command.DonorId, cancellationToken);
        if (donor is null || donor.IsDeleted)
            throw new KeyNotFoundException($"Donor with Id {command.DonorId} not found.");

        var statuses = await _statusRepository.GetAllAsync(cancellationToken);
        var scheduledStatus = statuses.FirstOrDefault(s => s.Code == "SCHEDULED");
        if (scheduledStatus is null)
            throw new InvalidOperationException("Scheduled status not configured.");

        var appointment = new DonationAppointment
        {
            DonorId = command.DonorId,
            BloodBankId = command.BloodBankId,
            StatusId = scheduledStatus.Id,
            AppointmentDate = command.AppointmentDate,
            Notes = command.Notes,
        };

        await _appointmentRepository.AddAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DonationAppointmentDto
        {
            Id = appointment.Id,
            DonorId = appointment.DonorId,
            DonorName = $"{donor.FirstName} {donor.LastName}",
            AppointmentDate = appointment.AppointmentDate,
            Status = scheduledStatus.Code,
            Notes = appointment.Notes,
        };
    }
}
