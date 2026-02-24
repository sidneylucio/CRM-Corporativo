namespace CRM.Corporativo.Infra.Data.Auth.Services.Jwt;

public class JwtTokenInfo
{
    public JwtTokenInfo(Guid userId, Guid accountId, string name, string email, IList<string> userRoles)
    {
        UserId = userId;
        AccountId = accountId;
        Name = name;
        Email = email;
        UserRoles = userRoles;
    }

    public Guid UserId { get; private set; }
    public Guid AccountId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public IList<string> UserRoles { get; private set; }
}
