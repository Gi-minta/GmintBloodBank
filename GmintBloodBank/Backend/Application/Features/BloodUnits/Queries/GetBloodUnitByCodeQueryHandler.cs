using Application.Common.CQRS;
using Application.DTOs.BloodUnits;
using Application.Interfaces.Persistence;
using Domain.Entities.BloodUnits;

namespace Application.Features.BloodUnits.Queries;

public sealed class GetBloodUnitByCodeQueryHandler : IQueryHandler<GetBloodUnitByCodeQuery, BloodUnitDto>
{
    private readonly IRepository<BloodUnit> _bloodUnitRepository;

    public GetBloodUnitByCodeQueryHandler(IRepository<BloodUnit> bloodUnitRepository)
    {
        _bloodUnitRepository = bloodUnitRepository;
    }

    public async Task<BloodUnitDto> HandleAsync(GetBloodUnitByCodeQuery query, CancellationToken cancellationToken = default)
    {
        var units = await _bloodUnitRepository.GetAllAsync(cancellationToken);
        var unit = units.FirstOrDefault(u =>
            u.UnitCode == query.UnitCode && !u.IsDeleted);

        if (unit is null)
            throw new KeyNotFoundException($"BloodUnit with code {query.UnitCode} not found.");

        return new BloodUnitDto
        {
            Id = unit.Id,
            UnitCode = unit.UnitCode,
            QrCode = unit.QrCode,
            BloodType = unit.BloodType?.Code ?? "",
            Component = unit.Component?.Name ?? "",
            Status = unit.Status?.Code ?? "",
            VolumeML = unit.VolumeML,
            CollectionDate = unit.CollectionDate,
            ExpirationDate = unit.ExpirationDate,
            IsReleased = unit.IsReleased,
        };
    }
}
