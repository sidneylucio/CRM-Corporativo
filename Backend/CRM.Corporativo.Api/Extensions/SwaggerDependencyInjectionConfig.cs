using CRM.Corporativo.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CRM.Corporativo.Api.Extensions;

public static class SwaggerDependencyInjectionConfig
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen(options =>
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = "CRM Corporativo API",
                    Version = description.ApiVersion.ToString(),
                    Description = "API do módulo de Clientes - CRM Corporativo"
                });
            }

            options.OperationFilter<SwaggerResponsesOperationFilter>();
            options.EnableAnnotations(true, true);
        });

        return services;
    }

    public static WebApplication UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in provider.ApiVersionDescriptions)
            {
                c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"CRM Corporativo API {description.ApiVersion}");
            }
            c.RoutePrefix = "swagger";
            c.ConfigObject.DisplayOperationId = false;
            c.ConfigObject.DocExpansion = DocExpansion.List;
            c.ConfigObject.DefaultModelsExpandDepth = 5;
        });

        return app;
    }
}
