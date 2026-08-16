using Samad.Domain.Entity;

namespace Samad.Infrastructure.IRepositories
{
    public interface IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : notnull
    {
        IQueryable<TEntity> Query();
        IQueryable<TEntity> QueryTracking();
        ValueTask<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        void DeleteRange(IEnumerable<TEntity> entities);
    }
}
