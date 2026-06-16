namespace Domain.Exceptions;

public sealed class FeatureNotLicensedException : DomainException
{
    public string FeatureKey { get; }

    public FeatureNotLicensedException(string featureKey)
        : base($"Feature '{featureKey}' is not licensed for this tenant.")
    {
        FeatureKey = featureKey;
    }
}
