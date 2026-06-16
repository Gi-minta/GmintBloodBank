namespace Domain.ValueObjects;

public sealed record DateRange(DateTime Start, DateTime End)
{
    public bool Contains(DateTime date) => date >= Start && date <= End;
    public bool Overlaps(DateRange other) => Start <= other.End && End >= other.Start;
}
