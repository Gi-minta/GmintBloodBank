using Domain.Entities.Common;

namespace Domain.Entities.Security;

public class AuditLog : Entity
{
    public Guid? UserId { get; set; }
    public string TableName { get; set; }
    public Guid RecordId { get; set; }
    public string Action { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
