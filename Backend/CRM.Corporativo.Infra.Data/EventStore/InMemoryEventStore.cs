using CRM.Corporativo.Domain.Models;
using CRM.Corporativo.Infra.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Corporativo.Infra.Data.EventStore;

public class InMemoryEventStore : IEventStore
{
    private readonly IApplicationContext _context;

    public InMemoryEventStore(IApplicationContext context)
    {
        _context = context;
    }

    public async Task AppendAsync(CustomerEvent customerEvent, CancellationToken cancellationToken)
    {
        customerEvent.Id = Guid.NewGuid();
        customerEvent.CreatedAt = DateTime.UtcNow;

        await _context.Set<CustomerEvent>().AddAsync(customerEvent, cancellationToken);
        await _context.SaveChanges(cancellationToken);
    }

    public Task<IEnumerable<CustomerEvent>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
        => _context.Set<CustomerEvent>()
            .Where(e => e.CustomerId == customerId)
            .OrderBy(e => e.OccurredAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IEnumerable<CustomerEvent>)t.Result, cancellationToken);
}
