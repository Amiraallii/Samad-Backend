using Samad.Infrastructure.Context;
using Samad.Infrastructure.IRepositories;

namespace Samad.Infrastructure.Repositories
{
    public class UnitOfWork(SamadDbContext _dbContext) : IUnitOfWork
    {

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
