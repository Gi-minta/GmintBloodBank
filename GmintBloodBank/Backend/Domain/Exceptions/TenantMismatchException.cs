namespace Domain.Exceptions;

public sealed class TenantMismatchException : DomainException
{
    public Guid ExpectedTenantId { get; }
    public Guid ActualTenantId { get; }

    public TenantMismatchException(Guid expectedTenantId, Guid actualTenantId)
        : base($"Tenant mismatch: expected {expectedTenantId}, actual {actualTenantId}.")
    {
        ExpectedTenantId = expectedTenantId;
        ActualTenantId = actualTenantId;
    }
}
