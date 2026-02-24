using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRM.Corporativo.Domain.Interfaces;
using CRM.Corporativo.Infra.Data.Auth.DTOs;
using CRM.Corporativo.Infra.Data.Auth.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Corporativo.Infra.Data.Auth.Services.Jwt;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _authSigningKey;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));
    }

    public AuthToken GenerateJwtToken(JwtTokenInfo tokenInfo)
    {
        DateTime dateTimeNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            claims: GetClaims(tokenInfo),
            expires: dateTimeNow.AddHours(8),
            signingCredentials: new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new AuthToken(Token: new JwtSecurityTokenHandler().WriteToken(token), Expiration: token.ValidTo);
    }

    private IEnumerable<Claim> GetClaims(JwtTokenInfo tokenInfo)
    {
        var claims = new List<Claim>()
        {
            new Claim("UserId", tokenInfo.UserId.ToString()),
            new Claim("AccountId", tokenInfo.AccountId.ToString()),
            new Claim("Name", tokenInfo.Name),
            new Claim("Email", tokenInfo.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(tokenInfo.UserRoles.Select(x => new Claim(ClaimTypes.Role, x)));

        return claims;
    }
}
