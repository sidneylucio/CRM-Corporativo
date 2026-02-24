using CRM.Corporativo.Application.DI;
using CRM.Corporativo.Domain.Base;
using CRM.Corporativo.Domain.Interfaces;
using CRM.Corporativo.Infra.Data.DI;
using CRM.Corporativo.Infra.Services.DI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Corporativo.Infra.IoC.DI;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddBackendTemplateDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IRequestInfo, RequestInfo>();

        services.AddApplicationDependencies();

        services.AddInfraDataServices((_, options) =>
        {
            options.UseInMemoryDatabase("CrmDb");
        });

        services.AddInfraServices(configuration);

        return services;
    }
}
