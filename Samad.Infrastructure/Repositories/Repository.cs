using Microsoft.EntityFrameworkCore;
using Samad.Domain.Entity;
using Samad.Infrastructure.Context;
using Samad.Infrastructure.IRepositories;

namespace Samad.Infrastructure.Repositories
{
    public class Repository<TEntity, TKey>(SamadDbContext dbContext) : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
    {
        private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();


        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);

        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public ValueTask<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
        {
            return _dbSet.FindAsync(
            [id],
            ct);
        }

        public IQueryable<TEntity> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<TEntity> QueryTracking()
        {
            return _dbSet;
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
    }
}
