using CRM.Corporativo.Domain.Base;
using CRM.Corporativo.Domain.Interfaces;
using CRM.Corporativo.Infra.Data.Auth.Context;
using CRM.Corporativo.Infra.Data.Auth.Interfaces;
using CRM.Corporativo.Infra.Data.Auth.Models;
using CRM.Corporativo.Infra.Data.Auth.Services;
using CRM.Corporativo.Infra.Data.Auth.Services.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Corporativo.Infra.Data.Auth.DI
{
    public static class IdentityDependencyInjector
    {
        public static IServiceCollection AddIdentityFramework(this IServiceCollection services, Action<DbContextOptionsBuilder>? optionsAction = null)
        {
            services.AddDbContext<IIdentityContext, IdentityContext>(optionsAction);

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton<IRequestInfo, RequestInfo>();
            services.AddSingleton<ITokenService, JwtTokenService>();

            return services;
        }
    }
}
