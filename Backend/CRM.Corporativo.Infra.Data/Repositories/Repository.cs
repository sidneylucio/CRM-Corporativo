using System.Linq.Expressions;
using CRM.Corporativo.Domain.Models;
using CRM.Corporativo.Domain.Models.Base;
using CRM.Corporativo.Infra.Data.Context;
using CRM.Corporativo.Infra.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Corporativo.Infra.Data.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly IApplicationContext _context;
    private readonly DbSet<TEntity> _entity;
    public IQueryable<TEntity> Entity => _entity;

    public Repository(IApplicationContext context)
    {
        _context = context;
        _entity = _context.Set<TEntity>();
    }

    public virtual Task<TEntity?> Get(Guid id, CancellationToken? cancellationToken)
    {
        return GetIncludes(id, cancellationToken);
    }

    public virtual Task<TEntity?> GetIncludes(Guid Id, CancellationToken? cancellationToken, IEnumerable<Expression<Func<TEntity, object>>>? includes = null)
    {
        return QueryFirst(x => x.Id!.Equals(Id), cancellationToken, includes);
    }

    public virtual async Task<TEntity?> QueryFirst(Expression<Func<TEntity, bool>> query, CancellationToken? cancellationToken, IEnumerable<Expression<Func<TEntity, object>>>? includes = null)
    {
        var entity = Entity;
        if (includes is not null)
            foreach (var include in includes)
                entity = entity.Include(include);

        var result = await entity.FirstOrDefaultAsync(query, cancellationToken ?? CancellationToken.None);

        return result;
    }

    public virtual Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> query, CancellationToken? cancellationToken, IEnumerable<Expression<Func<TEntity, object>>>? includes = null)
    {
        var entity = Entity;
        if (includes is not null)
            foreach (var include in includes)
                entity = entity.Include(include);

        return entity.Where(query).ToListAsync(cancellationToken ?? CancellationToken.None);
    }

    public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query, IEnumerable<Expression<Func<TEntity, object>>>? includes = null)
    {
        var entity = Entity;
        if (includes is not null)
            foreach (var include in includes)
                entity = entity.Include(include);

        return entity.Where(query);
    }

    public Task<IEnumerable<TEntity>> GetAll(CancellationToken? cancellationToken)
    {
        return GetAll(x => x.DeletedAt == null, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAll(Expression<Func<TEntity, bool>> query, CancellationToken? cancellationToken, IEnumerable<Expression<Func<TEntity, object>>>? includes = null)
    {
        var entities = _entity.AsNoTracking().Where(query);

        if (includes is not null)
            foreach (var include in includes)
                entities = entities.Include(include);

        return await entities.ToListAsync(cancellationToken ?? CancellationToken.None);
    }

    public virtual Task Insert(TEntity entity, CancellationToken? cancellationToken, bool autosave = true)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(TEntity));

        entity.CreatedAt = DateTime.UtcNow;
        _entity.AddAsync(entity, cancellationToken ?? CancellationToken.None);

        if (autosave) return _context.SaveChanges(cancellationToken ?? CancellationToken.None);

        return Task.FromResult(0);
    }

    public virtual Task Insert(IEnumerable<TEntity> entities, CancellationToken? cancellationToken, bool autosave = true)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(TEntity));

        entities = entities.Select(x => { x.CreatedAt = DateTime.UtcNow; return x; });
        _entity.AddRangeAsync(entities, cancellationToken ?? CancellationToken.None);

        if (autosave) return _context.SaveChanges(cancellationToken ?? CancellationToken.None);

        return Task.FromResult(0);
    }

    public virtual Task Update(TEntity entity, CancellationToken? cancellationToken, bool autosave = true)
    {
        if (_entity.FindAsync(entity.Id) is var result && result.Result == null)
            throw new ArgumentNullException(nameof(TEntity));

        entity.CreatedAt = result.Result.CreatedAt;
        entity.CreatedBy = result.Result.CreatedBy;

        _context.Entry(result.Result).CurrentValues.SetValues(entity);

        if (autosave) return _context.SaveChanges(cancellationToken ?? CancellationToken.None);

        return Task.FromResult(0);
    }

    public virtual Task Delete(Guid Id, CancellationToken? cancellationToken, bool autosave = true)
    {
        if (_entity.FindAsync(Id) is var result && result.Result == null)
            throw new ArgumentNullException(nameof(TEntity));

        _context.Entry(result.Result).State = EntityState.Deleted;

        if (autosave) return _context.SaveChanges(cancellationToken ?? CancellationToken.None);

        return Task.FromResult(0);
    }

    public Task SaveChanges(CancellationToken? cancellationToken)
    {
        return _context.SaveChanges(cancellationToken ?? CancellationToken.None);
    }
}
