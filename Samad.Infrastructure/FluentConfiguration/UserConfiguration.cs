using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Samad.Domain.Entity;

namespace Samad.Infrastructure.FluentConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.NationalCode).IsRequired().HasMaxLength(10).IsFixedLength();
            builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(11).IsFixedLength();
            builder.Property(u => u.Email).HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);

            builder.HasIndex(u => u.NationalCode).IsUnique();
            builder.HasIndex(u => u.PhoneNumber).IsUnique();
        }
    }
}
