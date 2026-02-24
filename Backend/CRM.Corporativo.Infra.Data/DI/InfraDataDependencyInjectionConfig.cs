using CRM.Corporativo.Infra.Data.Context;
using CRM.Corporativo.Infra.Data.EventStore;
using CRM.Corporativo.Infra.Data.Interceptors;
using CRM.Corporativo.Infra.Data.Interfaces;
using CRM.Corporativo.Infra.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Corporativo.Infra.Data.DI;

public static class InfraDataDependencyInjectionConfig
{
    public static void AddInfraDataServices(this IServiceCollection services, Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction = null)
    {
        services.AddSingleton<IEntityConfiguration, NullEntityConfiguration>();

        // AuditableEntitiesInterceptor depende de IRequestInfo (Singleton), portanto também é Singleton
        services.AddSingleton<AuditableEntitiesInterceptor>();

        services.AddDbContext<IApplicationContext, ApplicationContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntitiesInterceptor>();
            options.AddInterceptors(interceptor);
            optionsAction?.Invoke(sp, options);
        });

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IEventStore, InMemoryEventStore>();
    }

    public static async Task<IApplicationBuilder> ApplyApplicationMigrations(this IApplicationBuilder app, AsyncServiceScope scope)
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        await context.Database.EnsureCreatedAsync();
        await context.SeedData();
        return app;
    }
}

internal sealed class NullEntityConfiguration : IEntityConfiguration
{
    public void Configure(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder) { }
}
