using CRM.Corporativo.Domain.Enums;

namespace CRM.Corporativo.Domain.Commands.Customer;

public record CreateCustomerCommand(
    string Name,
    string Document,
    CustomerTypeEnum CustomerType,
    DateTime BirthDate,
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
