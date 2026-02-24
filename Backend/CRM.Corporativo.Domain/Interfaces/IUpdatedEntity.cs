namespace CRM.Corporativo.Domain.Interfaces;
public interface IUpdatedEntity
{
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}
