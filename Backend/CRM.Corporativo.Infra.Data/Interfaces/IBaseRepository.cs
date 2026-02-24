using CRM.Corporativo.Domain.Interfaces;

namespace CRM.Corporativo.Infra.Data.Interfaces;

public interface IBaseRepository<T> where T : IEntity
{
    Task<T?> Get(Guid id, CancellationToken? cancellationToken);

    Task<IEnumerable<T>> GetAll(CancellationToken? cancellationToken);

    Task Insert(T entity, CancellationToken? cancellationToken, bool autosave = true);

    Task Insert(IEnumerable<T> entities, CancellationToken? cancellationToken, bool autosave = true);

    Task Update(T entity, CancellationToken? cancellationToken, bool autosave = true);

    Task Delete(Guid id, CancellationToken? cancellationToken, bool autosave = true);
}
