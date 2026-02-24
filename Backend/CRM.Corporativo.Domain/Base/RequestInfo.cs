using CRM.Corporativo.Domain.Interfaces;

namespace CRM.Corporativo.Domain.Base;

public class RequestInfo : IRequestInfo
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }

    public void SetUserInfo(Guid userId, Guid accountId, string? name, string? email)
    {
        UserId = userId;
        AccountId = accountId;
        Name = name;
        Email = email;
    }
}