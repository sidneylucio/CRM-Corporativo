using CRM.Corporativo.Infra.Data.Interfaces;
using CRM.Corporativo.Infra.Data.Migrations.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Corporativo.Infra.Data.Migrations.DI;

public static class DependencyInjector
{
    public static IServiceCollection AddMigrations(this IServiceCollection services)
    {
        services.AddScoped<IEntityConfiguration, EntityConfiguration>();

        return services;
    }
}