using Application.Common.CQRS;
using Application.DTOs.BloodUnits;

namespace Application.Features.BloodUnits.Queries;

public record GetBloodUnitByCodeQuery(string UnitCode) : IQuery<BloodUnitDto>;
