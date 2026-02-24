using System.Security.Claims;
using CRM.Corporativo.Infra.Data.Auth.DTOs;
using CRM.Corporativo.Infra.Data.Auth.Services.Jwt;

namespace CRM.Corporativo.Infra.Data.Auth.Interfaces;

public interface ITokenService
{
    AuthToken GenerateJwtToken(JwtTokenInfo tokenInfo);
}
