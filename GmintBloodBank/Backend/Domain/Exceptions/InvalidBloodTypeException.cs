namespace Domain.Exceptions;

public sealed class InvalidBloodTypeException : DomainException
{
    public InvalidBloodTypeException(string bloodType)
        : base($"The blood type '{bloodType}' is not valid.") { }
}
