namespace CRM.Corporativo.Domain.Commands.Customer;

public record CustomerEventResponse(
    Guid Id,
    Guid CustomerId,
    string EventType,
    string Payload,
    DateTime OccurredAt,
    string OccurredBy);
