using CRM.Corporativo.Domain.Models;
using CRM.Corporativo.Domain.Models.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Corporativo.Infra.Data.Migrations.Extensions;
public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<TEntity> ConfigureDefaults<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : Entity
    {
        builder.HasKey(b => b.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.Property(t => t.CreatedAt);
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.DeletedAt);

        return builder;
    }
}