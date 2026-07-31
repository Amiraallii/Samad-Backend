using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Samad.Domain.Entity;

namespace Samad.Infrastructure.FluentConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Title).IsRequired().HasMaxLength(50);

            builder.HasData(
                new Role { Id = 1, Title = "Applicant" },
                new Role { Id = 2, Title = "CouncilMember" },
                new Role { Id = 3, Title = "Secretary" },
                new Role { Id = 4, Title = "Admin" }
            );
        }
    }
}
