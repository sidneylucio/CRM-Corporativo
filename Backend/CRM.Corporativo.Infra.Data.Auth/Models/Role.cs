using Microsoft.AspNetCore.Identity;

namespace CRM.Corporativo.Infra.Data.Auth.Models;

public class Role : IdentityRole<Guid>
{
    public Role()
    {
    }

    public Role(string roleName, string description) : base(roleName)
    {
        Description = description;
    }

    public string Description { get; set; }
}
