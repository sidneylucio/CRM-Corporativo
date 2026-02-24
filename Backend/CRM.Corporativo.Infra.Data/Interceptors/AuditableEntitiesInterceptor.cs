using CRM.Corporativo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CRM.Corporativo.Infra.Data.Interceptors;

public class AuditableEntitiesInterceptor : SaveChangesInterceptor
{
    private readonly IRequestInfo _requestInfo;

    public AuditableEntitiesInterceptor(IRequestInfo requestInfo)
    {
        _requestInfo = requestInfo;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        var user = _requestInfo?.Name ?? "System";

        if (context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var entry in context.ChangeTracker.Entries<IEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property("CreatedAt").CurrentValue ??= DateTime.UtcNow;
                    entry.Property("CreatedBy").CurrentValue ??= user;
                    break;
                case EntityState.Modified:
                    entry.Property("UpdatedAt").CurrentValue ??= DateTime.UtcNow;
                    entry.Property("UpdatedBy").CurrentValue ??= user;
                    break;
                case EntityState.Deleted:
                    entry.Property("DeletedAt").CurrentValue ??= DateTime.UtcNow;
                    entry.Property("DeletedBy").CurrentValue ??= user;
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}