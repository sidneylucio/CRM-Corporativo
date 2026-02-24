using CRM.Corporativo.Domain.Enums;

namespace CRM.Corporativo.Domain.Commands.Customer;

public record CustomerResponse(
    Guid Id,
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
    bool IsStateRegistrationExempt,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
