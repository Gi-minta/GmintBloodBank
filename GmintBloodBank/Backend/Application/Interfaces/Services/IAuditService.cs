namespace Application.Interfaces.Services;

public interface IAuditService
{
    Task LogAsync(string tableName, Guid recordId, string action, string? oldValues = null, string? newValues = null, CancellationToken cancellationToken = default);
}
