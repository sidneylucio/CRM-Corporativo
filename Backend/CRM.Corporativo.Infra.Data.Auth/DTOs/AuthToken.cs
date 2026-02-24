namespace CRM.Corporativo.Infra.Data.Auth.DTOs;

public sealed record AuthToken(string Token, DateTime Expiration);