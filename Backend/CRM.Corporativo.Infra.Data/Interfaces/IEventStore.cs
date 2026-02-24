using CRM.Corporativo.Domain.Models;

namespace CRM.Corporativo.Infra.Data.Interfaces;

public interface IEventStore
{
    Task AppendAsync(CustomerEvent customerEvent, CancellationToken cancellationToken);
    Task<IEnumerable<CustomerEvent>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
}
