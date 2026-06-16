namespace Domain.Exceptions;

public sealed class BloodUnitExpiredException : DomainException
{
    public Guid BloodUnitId { get; }

    public BloodUnitExpiredException(Guid bloodUnitId)
        : base($"Blood unit {bloodUnitId} has expired.")
    {
        BloodUnitId = bloodUnitId;
    }
}
