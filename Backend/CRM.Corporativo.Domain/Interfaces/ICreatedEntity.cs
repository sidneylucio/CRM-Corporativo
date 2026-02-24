namespace CRM.Corporativo.Domain.Interfaces;
public interface ICreatedEntity
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
}
