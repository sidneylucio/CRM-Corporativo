using CRM.Corporativo.Domain.Base;
using CRM.Corporativo.Domain.Commands.Customer;

namespace CRM.Corporativo.Application.Services
{
    public interface ICustomerService
    {
        Task<Result<CustomerResponse>> CreateAsync(CreateCustomerCommand command, CancellationToken cancellationToken);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<Result<List<CustomerResponse>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<CustomerResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Result<List<CustomerEventResponse>>> GetEventsAsync(Guid customerId, CancellationToken cancellationToken);
        Task<Result<CustomerResponse>> UpdateAsync(UpdateCustomerCommand command, CancellationToken cancellationToken);
    }
}