using Microsoft.EntityFrameworkCore;
using Samad.Domain.Entity;
namespace Samad.Infrastructure.Context
{
    public class SamadDbContext : DbContext
    {
        public SamadDbContext(DbContextOptions<SamadDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestDocument> RequestDocuments { get; set; }
        public DbSet<CouncilReview> CouncilReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SamadDbContext).Assembly);
        }
    }
}
