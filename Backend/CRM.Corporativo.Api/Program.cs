using CRM.Corporativo.Api.Extensions;
using CRM.Corporativo.Api.Filters;
using CRM.Corporativo.Api.Middlewares;
using CRM.Corporativo.Infra.Data.DI;
using CRM.Corporativo.Infra.IoC.DI;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Globalization;

namespace CRM.Corporativo.Api;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, services, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
                         .ReadFrom.Services(services)
                         .Enrich.FromLogContext());

        builder.Services.AddSwaggerServices();
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add(new ValidationExceptionFilter());
        });

        builder.Services.AddBackendTemplateDependencies(builder.Configuration);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(corsBuilder =>
            {
                corsBuilder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            await using var scope = app.Services.CreateAsyncScope();
            await app.ApplyApplicationMigrations(scope);
        }

        var cultureInfo = new CultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        app.UseCustomSwagger();
        app.UseCors();
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.MapControllers();
        app.UseMiddleware<ExceptionMiddleware>();
        await app.RunAsync();
    }
}
