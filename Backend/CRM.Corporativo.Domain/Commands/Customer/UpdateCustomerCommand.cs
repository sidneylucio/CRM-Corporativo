namespace CRM.Corporativo.Domain.Commands.Customer;

public record UpdateCustomerCommand(
    Guid Id,
    string Name,
    string Phone,
    string Email,
    string ZipCode,
    string Street,
    string Number,
    string Neighborhood,
    string City,
    string State,
    string? StateRegistration,
    bool IsStateRegistrationExempt = false);
