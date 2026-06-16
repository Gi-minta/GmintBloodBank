namespace Application.DTOs.Licensing;

public class FeatureFlagDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public string Scope { get; set; } = string.Empty;
}
