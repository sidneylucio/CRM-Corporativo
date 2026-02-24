using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Corporativo.Infra.Data.Auth.Extensions;

public static class PoliciesConfiguration
{
    public static void AddCustomAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("AdminsAndAnalysts", policy =>
                policy.Requirements.Add(new RolesAuthorizationRequirement(["Admin, Analyst"])))
            .AddPolicy("OnlyAdmins", policy =>
                policy.RequireRole("Admin"));
    }
}
