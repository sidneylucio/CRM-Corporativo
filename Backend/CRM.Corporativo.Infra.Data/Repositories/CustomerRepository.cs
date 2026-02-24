using CRM.Corporativo.Domain.Models;
using CRM.Corporativo.Infra.Data.Context;
using CRM.Corporativo.Infra.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Corporativo.Infra.Data.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(IApplicationContext context) : base(context)
    {
    }

    public Task<Customer?> GetByDocumentAsync(string document, CancellationToken cancellationToken)
        => Entity
            .FirstOrDefaultAsync(x => x.Document == document && x.DeletedAt == null, cancellationToken);

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => Entity
            .FirstOrDefaultAsync(x => x.Email == email && x.DeletedAt == null, cancellationToken);
}
