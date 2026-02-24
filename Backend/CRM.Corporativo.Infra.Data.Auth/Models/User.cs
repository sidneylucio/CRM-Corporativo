using Microsoft.AspNetCore.Identity;

namespace CRM.Corporativo.Infra.Data.Auth.Models;
public class User : IdentityUser<Guid>
{
    public string Name { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
}
