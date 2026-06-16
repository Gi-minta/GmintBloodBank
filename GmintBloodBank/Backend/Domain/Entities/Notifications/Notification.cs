using Domain.Entities.Common;
using Domain.Entities.Security;

namespace Domain.Entities.Notifications;

public class Notification : AuditableEntity
{
    public Guid TypeId { get; set; }
    public Guid? UserId { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? SentAt { get; set; }
    public NotificationType Type { get; set; }
    public User? User { get; set; }
}
