namespace Domain.ValueObjects;

// Value object for email. Constructor validates basic format (contains '@').
public sealed record Email
{
    public string Value { get; }
    
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            throw new ArgumentException("Invalid email format.", nameof(value));
        Value = value;
    }
    
    public void Deconstruct(out string value) => value = Value;
}
