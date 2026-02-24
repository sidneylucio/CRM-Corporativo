using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CRM.Corporativo.Infra.CrossCutting.DI;

public static class CrossCuttingDependencyInjectionConfig
{
    public static IServiceCollection AddCrossCuttingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationInsightsTelemetry();

        var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
        telemetryConfiguration.ConnectionString = configuration["ApplicationInsights:ConnectionString"];

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
            .CreateLogger();

        services.AddSerilog(Log.Logger);

        return services;
    }

    public static WebApplication UseSerilog(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}
