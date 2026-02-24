using CRM.Corporativo.Domain.Models;
using CRM.Corporativo.Infra.Data.Migrations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Corporativo.Infra.Data.Migrations.Configuration;

public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ConfigureDefaults();

        builder.Property(x => x.Status).IsRequired();

        builder
            .HasOne(ua => ua.User)
            .WithMany(a => a.UserAccounts)
            .HasForeignKey(ua => ua.UserId);
    }
}
