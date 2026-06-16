namespace Domain.Exceptions;

public sealed class DonorNotEligibleException : DomainException
{
    public Guid DonorId { get; }

    public DonorNotEligibleException(Guid donorId)
        : base($"Donor {donorId} is not eligible for donation.")
    {
        DonorId = donorId;
    }
}
