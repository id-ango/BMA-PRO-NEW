using Accounting.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Accounting.Services;

public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly AuditService _auditService;
    private readonly AuditContext _auditContext;

    public AuditSaveChangesInterceptor(AuditService auditService, AuditContext auditContext)
    {
        _auditService = auditService;
        _auditContext = auditContext;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (!_auditContext.SuppressAudit && eventData.Context is not null)
        {
            foreach (var entry in eventData.Context.ChangeTracker.Entries()
                         .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
            {
                var action = entry.State switch
                {
                    EntityState.Added => "Create",
                    EntityState.Modified => "Update",
                    EntityState.Deleted => "Delete",
                    _ => null
                };

                if (action is null)
                {
                    continue;
                }

                var values = entry.Properties
                    .Where(property => !IsSensitive(property.Metadata.Name))
                    .Where(property => entry.State != EntityState.Modified || property.IsModified)
                    .ToDictionary(property => property.Metadata.Name, property => property.CurrentValue);

                var key = string.Join(",", entry.Properties
                    .Where(property => property.Metadata.IsPrimaryKey())
                    .Select(property => $"{property.Metadata.Name}={property.CurrentValue}"));

                await _auditService.WriteAsync(
                    action,
                    entry.Metadata.ClrType.Name,
                    key,
                    values,
                    cancellationToken);
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static bool IsSensitive(string name) =>
        name.Contains("password", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("token", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("secret", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("hash", StringComparison.OrdinalIgnoreCase);
}
