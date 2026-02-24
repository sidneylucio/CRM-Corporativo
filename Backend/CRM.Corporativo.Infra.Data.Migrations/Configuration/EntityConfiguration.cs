using CRM.Corporativo.Domain.Models;
using CRM.Corporativo.Infra.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Corporativo.Infra.Data.Migrations.Configuration;

public class EntityConfiguration : IEntityConfiguration
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<User>();
        modelBuilder.Entity<User>().ToTable(nameof(User), t => t.ExcludeFromMigrations());

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EntityConfiguration).Assembly);
    }
}
