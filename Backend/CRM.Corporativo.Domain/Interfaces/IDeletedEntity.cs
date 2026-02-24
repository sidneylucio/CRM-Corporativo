namespace CRM.Corporativo.Domain.Interfaces;
public interface IDeletedEntity
{
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}
