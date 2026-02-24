using CRM.Corporativo.Domain.Models;
using CRM.Corporativo.Infra.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Corporativo.Infra.Data.Context;

public class ApplicationContext : DbContext, IApplicationContext
{
    private readonly IEntityConfiguration? _configuration;

    public ApplicationContext(DbContextOptions<ApplicationContext> options, IEntityConfiguration? configuration) : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerEvent> CustomerEvents => Set<CustomerEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _configuration?.Configure(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public async Task SeedData()
    {
        await Task.CompletedTask;
    }
}
