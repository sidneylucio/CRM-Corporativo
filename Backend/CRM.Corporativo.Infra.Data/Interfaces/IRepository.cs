using CRM.Corporativo.Domain.Models.Base;
using System.Linq.Expressions;

namespace CRM.Corporativo.Infra.Data.Interfaces;

public interface IRepository<T> : IBaseRepository<T> where T : Entity
{
    public IQueryable<T> Entity { get; }

    Task<T?> GetIncludes(Guid Id, CancellationToken? cancellationToken, IEnumerable<Expression<Func<T, object>>>? includes = null);

    Task<T?> QueryFirst(Expression<Func<T, bool>> query, CancellationToken? cancellationToken, IEnumerable<Expression<Func<T, object>>>? includes = null);

    Task<List<T>> Query(Expression<Func<T, bool>> query, CancellationToken? cancellationToken, IEnumerable<Expression<Func<T, object>>>? includes = null);

    IQueryable<T> Query(Expression<Func<T, bool>> query, IEnumerable<Expression<Func<T, object>>>? includes = null);

    Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> query, CancellationToken? cancellationToken, IEnumerable<Expression<Func<T, object>>>? includes = null);

    Task SaveChanges(CancellationToken? cancellationToken);
}