using CRM.Corporativo.Infra.Data.Auth.Interfaces;
using CRM.Corporativo.Infra.Data.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Corporativo.Infra.Data.Auth.Context;

public class IdentityContext : IdentityDbContext<User, Role, Guid>, IIdentityContext
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("User");
        builder.Entity<Role>().ToTable("Role");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRole").HasKey(p => new { p.UserId, p.RoleId });
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaim");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogin").HasKey(p => new { p.LoginProvider, p.ProviderKey });
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserToken").HasKey(p => new { p.UserId, p.LoginProvider, p.Name });
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaim");
    }

    public async Task SeedData(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            var rolesWithDescriptions = new Dictionary<string, string>
            {
                { "Admin", "DevFull administrator" },
                { "User", "Standard user with limited access" },
                { "Analyst", "DevFull analyst" },
                { "Dispatcher", "DevFull Dispatcher" },
                { "Viewer", "DevFull viewer" },
            };

            foreach (var role in rolesWithDescriptions)
            {
                if (await roleManager.RoleExistsAsync(role.Key))
                {
                    continue;
                }

                var newRole = new Role(role.Key, role.Value);
                await roleManager.CreateAsync(newRole);
            }

            if (await userManager.FindByEmailAsync("dev@devfull.com.br") == null)
            {
                var user = new User
                {
                    Id = Guid.Parse("88888888-8888-4888-8888-888888888888"),
                    ConcurrencyStamp = "e12befb8-837d-41a4-b48f-becabb40d993",
                    Name = "Admin - DevFull",
                    UserName = "dev@devfull.com.br",
                    Email = "dev@devfull.com.br",
                    EmailConfirmed = true,
                    CreatedBy = "System",
                    CreatedAt = DateTime.Parse("01/01/2025")
                };

                var result = await userManager.CreateAsync(user, "DevFull@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    await userManager.AddToRoleAsync(user, "Analyst");
                }
            }
        }
    }
}
