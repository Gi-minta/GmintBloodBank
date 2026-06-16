namespace Domain.ValueObjects;

public sealed record AddressVO(
    string Country,
    string? State,
    string City,
    string AddressLine1,
    string? AddressLine2,
    string? PostalCode,
    decimal? Latitude,
    decimal? Longitude);
