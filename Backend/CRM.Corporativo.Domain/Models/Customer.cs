using CRM.Corporativo.Domain.Enums;
using CRM.Corporativo.Domain.Models.Base;

namespace CRM.Corporativo.Domain.Models;

public class Customer : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public CustomerTypeEnum CustomerType { get; set; }
    public DateTime BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string? StateRegistration { get; set; }
    public bool IsStateRegistrationExempt { get; set; }
}
