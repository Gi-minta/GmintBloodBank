using Application.Interfaces.Services;
using Infrastructure.Persistence;

namespace Infrastructure.Services;

public sealed class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string tableName, Guid recordId, string action, string? oldValues = null, string? newValues = null, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual audit logging
        await Task.CompletedTask;
    }
}
