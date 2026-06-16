using Application.DTOs.Donors;
using Domain.Entities.Donors;

namespace Application.Common.Mappings;

public sealed class DonorMapper : IMapper<Donor, DonorDto>
{
    public DonorDto ToDto(Donor entity)
    {
        return new DonorDto
        {
            Id               = entity.Id,
            DonorCode        = entity.DonorCode,
            FirstName        = entity.FirstName,
            LastName         = entity.LastName,
            Identification   = entity.Identification,
            DateOfBirth      = entity.DateOfBirth,
            PhoneNumber      = entity.PhoneNumber,
            Email            = entity.Email,
            BloodTypeId      = entity.BloodTypeId,
            GenderId         = entity.GenderId,
            IsEligible       = entity.IsEligible,
            LastDonationDate = entity.LastDonationDate
        };
    }

    public Donor ToEntity(DonorDto dto)
    {
        return new Donor
        {
            DonorCode        = dto.DonorCode,
            FirstName        = dto.FirstName,
            LastName         = dto.LastName,
            Identification   = dto.Identification,
            DateOfBirth      = dto.DateOfBirth,
            PhoneNumber      = dto.PhoneNumber,
            Email            = dto.Email,
            BloodTypeId      = dto.BloodTypeId,
            GenderId         = dto.GenderId,
            IsEligible       = dto.IsEligible,
            LastDonationDate = dto.LastDonationDate
        };
    }
}
