using System.Security.Claims;
using System.Text;
using CRM.Corporativo.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Corporativo.Infra.Data.Auth.Extensions;

public static class AuthenticationConfiguration
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    RequireSignedTokens = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Authentication success: ");

                        var claims = context.Principal?.Claims;
                        if (claims == null)
                        {
                            context.Fail("User token has no claims");
                            return Task.CompletedTask;
                        }

                        var userId = claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                        var accountId = claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
                        var name = claims.FirstOrDefault(c => c.Type == "Name")?.Value;
                        var email = claims.FirstOrDefault(c => c.Type == "Email")?.Value;

                        var requestInfo = context.HttpContext.RequestServices.GetRequiredService<IRequestInfo>();
                        requestInfo.SetUserInfo(
                            Guid.Parse(userId ?? Guid.Empty.ToString()),
                            Guid.Parse(accountId ?? Guid.Empty.ToString()),
                            name,
                            email
                        );

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Authentication failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            })
            .AddMicrosoftIdentityWebApi(options =>
                {
                    configuration.Bind("AzureAd", options);
                    options.TokenValidationParameters.NameClaimType = "name";
                },
                options => { configuration.Bind("AzureAd", options); },
                "AzureAd");
    }
}