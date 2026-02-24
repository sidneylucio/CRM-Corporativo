namespace CRM.Corporativo.Domain.Interfaces;

public interface IRequestInfo
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    string? Name { get; set; }
    string? Email { get; set; }
    void SetUserInfo(Guid userId, Guid accountId, string? name, string? email);
}