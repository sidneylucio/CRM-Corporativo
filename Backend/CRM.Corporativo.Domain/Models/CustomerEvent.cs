using CRM.Corporativo.Domain.Models.Base;

namespace CRM.Corporativo.Domain.Models;

public class CustomerEvent : Entity
{
    public Guid CustomerId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string OccurredBy { get; set; } = string.Empty;
}
