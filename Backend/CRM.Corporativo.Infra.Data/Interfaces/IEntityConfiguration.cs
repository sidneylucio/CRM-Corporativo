using Microsoft.EntityFrameworkCore;

namespace CRM.Corporativo.Infra.Data.Interfaces;

public interface IEntityConfiguration
{
    void Configure(ModelBuilder modelBuilder);
}
