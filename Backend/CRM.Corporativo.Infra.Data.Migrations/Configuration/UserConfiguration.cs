using CRM.Corporativo.Domain.Models;
using CRM.Corporativo.Infra.Data.Migrations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Corporativo.Infra.Data.Migrations.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ConfigureDefaults();

        builder.Property(x => x.Username)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasMaxLength(256)
            .IsRequired();
    }
}
