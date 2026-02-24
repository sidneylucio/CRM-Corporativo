using CRM.Corporativo.Domain.Attributes;
using CRM.Corporativo.Domain.Interfaces;
using CRM.Corporativo.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CRM.Corporativo.Infra.Data.Interceptors
{
    public class TrackableEntitiesInterceptor : SaveChangesInterceptor
    {
        private readonly IRequestInfo _requestInfo;

        public TrackableEntitiesInterceptor(IRequestInfo requestInfo)
        {
            _requestInfo = requestInfo;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var dbContext = eventData.Context;
            if (dbContext == null)
                return await base.SavingChangesAsync(eventData, result, cancellationToken);


            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static async Task<string?> FetchTrackingKeyAsync(
            DbContext dbContext,
            Type entityType,
            Guid id,
            CancellationToken cancellationToken)
        {
            var instance = await dbContext.FindAsync(entityType, [id], cancellationToken);

            var keyProp = instance?.GetType()
                .GetProperties()
                .FirstOrDefault(p => p.IsDefined(typeof(TrackingKeyAttribute), false));

            return keyProp?.GetValue(instance)?.ToString();
        }
    }
}