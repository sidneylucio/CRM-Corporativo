using CRM.Corporativo.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Corporativo.Worker.DI;

public static class WorkerDependencyInjector
{
    public static void AddWorkerDependencyInjector(this IServiceCollection services)
    {
        services.AddHostedService<Worker>();
    }
}