using CRM.Corporativo.Domain.Base;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CRM.Corporativo.Api.Filters;

public class SwaggerResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!operation.Responses.ContainsKey(StatusCodes.Status400BadRequest.ToString()))
        {
            operation.Responses[StatusCodes.Status400BadRequest.ToString()] = new OpenApiResponse
            {
                Description = "Bad Request",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(IList<TError>), context.SchemaRepository)
                    }
                }
            };
        }
    }
}
