using CRM.Corporativo.Infra.Data.Auth.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Corporativo.Infra.Data.Auth.Extensions;

public static class IdentityMigrations
{
    public static async Task<IApplicationBuilder> ApplyAuthMigrations(this IApplicationBuilder app, AsyncServiceScope scope)
    {
        var authContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        await authContext.Database.MigrateAsync();
        await authContext.SeedData(scope.ServiceProvider);
        return app;
    }
}