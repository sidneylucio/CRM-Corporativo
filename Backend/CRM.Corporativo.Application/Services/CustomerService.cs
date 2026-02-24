using CRM.Corporativo.Domain.Base;
using CRM.Corporativo.Domain.Commands.Customer;
using CRM.Corporativo.Domain.Interfaces;
using CRM.Corporativo.Domain.Models;
using CRM.Corporativo.Domain.Services;
using CRM.Corporativo.Infra.Data.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CRM.Corporativo.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventStore _eventStore;
    private readonly IViaCepService _viaCepService;
    private readonly IRequestInfo _requestInfo;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepository,
        IEventStore eventStore,
        IViaCepService viaCepService,
        IRequestInfo requestInfo,
        ILogger<CustomerService> logger)
    {
        _customerRepository = customerRepository;
        _eventStore = eventStore;
        _viaCepService = viaCepService;
        _requestInfo = requestInfo;
        _logger = logger;
    }

    public async Task<Result<CustomerResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando cliente: {CustomerId}", id);

        var customer = await _customerRepository.Get(id, cancellationToken);

        if (customer is null)
            return Result.Fail<CustomerResponse>("Customer.NotFound", "Cliente não encontrado");

        return Result.Success(MapToResponse(customer));
    }

    public async Task<Result<List<CustomerResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando todos os clientes");

        var customers = await _customerRepository.GetAll(x => x.DeletedAt == null, cancellationToken);

        return Result.Success(customers.Select(MapToResponse).ToList());
    }

    public async Task<Result<CustomerResponse>> CreateAsync(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var actor = _requestInfo.Name ?? "System";

        _logger.LogInformation("Criando cliente: {CustomerName} por {Actor}", command.Name, actor);

        var normalizedDocument = NormalizeDocument(command.Document);

        var duplicateDoc = await _customerRepository.GetByDocumentAsync(normalizedDocument, cancellationToken);
        if (duplicateDoc is not null)
            return Result.Fail<CustomerResponse>("Customer.DuplicateDocument", "Já existe um cliente com este CPF/CNPJ");

        var duplicateEmail = await _customerRepository.GetByEmailAsync(command.Email.ToLower(), cancellationToken);
        if (duplicateEmail is not null)
            return Result.Fail<CustomerResponse>("Customer.DuplicateEmail", "Já existe um cliente com este e-mail");

        var address = await EnrichAddressAsync(command.ZipCode, command.Street, command.Neighborhood, command.City, command.State);

        var customer = new Customer
        {
            Name = command.Name,
            Document = normalizedDocument,
            CustomerType = command.CustomerType,
            BirthDate = command.BirthDate,
            Phone = command.Phone,
            Email = command.Email.ToLower(),
            ZipCode = address.ZipCode,
            Street = address.Street,
            Number = command.Number,
            Neighborhood = address.Neighborhood,
            City = address.City,
            State = address.State,
            StateRegistration = command.StateRegistration,
            IsStateRegistrationExempt = command.IsStateRegistrationExempt,
            CreatedBy = actor
        };

        await _customerRepository.Insert(customer, cancellationToken);

        await AppendEventAsync(customer.Id, "CustomerCreated", customer, actor, cancellationToken);

        return Result.Success(MapToResponse(customer));
    }

    public async Task<Result<CustomerResponse>> UpdateAsync(UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        var actor = _requestInfo.Name ?? "System";

        _logger.LogInformation("Atualizando cliente: {CustomerId} por {Actor}", command.Id, actor);

        var customer = await _customerRepository.Get(command.Id, cancellationToken);

        if (customer is null)
            return Result.Fail<CustomerResponse>("Customer.NotFound", "Cliente não encontrado");

        var duplicateEmail = await _customerRepository.GetByEmailAsync(command.Email.ToLower(), cancellationToken);
        if (duplicateEmail is not null && duplicateEmail.Id != command.Id)
            return Result.Fail<CustomerResponse>("Customer.DuplicateEmail", "Já existe um cliente com este e-mail");

        var address = await EnrichAddressAsync(command.ZipCode, command.Street, command.Neighborhood, command.City, command.State);

        customer.Name = command.Name;
        customer.Phone = command.Phone;
        customer.Email = command.Email.ToLower();
        customer.ZipCode = address.ZipCode;
        customer.Street = address.Street;
        customer.Number = command.Number;
        customer.Neighborhood = address.Neighborhood;
        customer.City = address.City;
        customer.State = address.State;
        customer.StateRegistration = command.StateRegistration;
        customer.IsStateRegistrationExempt = command.IsStateRegistrationExempt;
        customer.UpdatedBy = actor;
        customer.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.Update(customer, cancellationToken);

        await AppendEventAsync(customer.Id, "CustomerUpdated", customer, actor, cancellationToken);

        return Result.Success(MapToResponse(customer));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var actor = _requestInfo.Name ?? "System";

        _logger.LogInformation("Removendo cliente: {CustomerId} por {Actor}", id, actor);

        var customer = await _customerRepository.Get(id, cancellationToken);

        if (customer is null)
            return Result.Fail("Customer.NotFound", "Cliente não encontrado");

        customer.DeletedBy = actor;
        customer.DeletedAt = DateTime.UtcNow;

        await _customerRepository.Update(customer, cancellationToken);

        await AppendEventAsync(id, "CustomerDeleted", new { id, deletedBy = actor }, actor, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<List<CustomerEventResponse>>> GetEventsAsync(Guid customerId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando eventos do cliente: {CustomerId}", customerId);

        var events = await _eventStore.GetByCustomerIdAsync(customerId, cancellationToken);

        var response = events.Select(e => new CustomerEventResponse(
            e.Id,
            e.CustomerId,
            e.EventType,
            e.Payload,
            e.OccurredAt,
            e.OccurredBy)).ToList();

        return Result.Success(response);
    }

    private async Task AppendEventAsync(Guid customerId, string eventType, object payload, string occurredBy, CancellationToken cancellationToken)
    {
        var customerEvent = new CustomerEvent
        {
            CustomerId = customerId,
            EventType = eventType,
            Payload = JsonSerializer.Serialize(payload),
            OccurredAt = DateTime.UtcNow,
            OccurredBy = occurredBy,
            CreatedBy = occurredBy
        };

        await _eventStore.AppendAsync(customerEvent, cancellationToken);
    }

    private async Task<(string ZipCode, string Street, string Neighborhood, string City, string State)> EnrichAddressAsync(
        string zipCode, string street, string neighborhood, string city, string state)
    {
        try
        {
            var address = await _viaCepService.GetAddressAsync(zipCode);
            if (address is not null)
            {
                return (
                    address.Cep ?? zipCode,
                    string.IsNullOrWhiteSpace(address.Logradouro) ? street : address.Logradouro,
                    string.IsNullOrWhiteSpace(address.Bairro) ? neighborhood : address.Bairro,
                    string.IsNullOrWhiteSpace(address.Localidade) ? city : address.Localidade,
                    string.IsNullOrWhiteSpace(address.Uf) ? state : address.Uf
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao consultar ViaCEP para o CEP {ZipCode}. Usando dados informados.", zipCode);
        }

        return (zipCode, street, neighborhood, city, state);
    }

    private static string NormalizeDocument(string document)
        => new string(document.Where(char.IsDigit).ToArray());

    private static CustomerResponse MapToResponse(Customer customer) =>
        new(customer.Id,
            customer.Name,
            customer.Document,
            customer.CustomerType,
            customer.BirthDate,
            customer.Phone,
            customer.Email,
            customer.ZipCode,
            customer.Street,
            customer.Number,
            customer.Neighborhood,
            customer.City,
            customer.State,
            customer.StateRegistration,
            customer.IsStateRegistrationExempt,
            customer.CreatedAt,
            customer.UpdatedAt);
}
