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
            builder.HasData(new User
            {
                Id = 1,
                BirthDate = new DateTime(2000, 7, 27),
                CreatedAt = new DateTime(2000, 7, 27),
                Email = "amiraliaghaeibs@gmail.com",
                FirstName = "Amirali",
                LastName = "Aghaei",
                RoleId = 4,
                PhoneNumber = "09120882458",
                NationalCode = "2581288426",
                PasswordHash = "AQAAAAIAAYagAAAAEB9aIlLnvh557v9Jn7OQBBTJIIbZC1gA5RVkFD+JCrl65djhtNZWAo2Bn9at/aLkVA=="
            });
        }
    }
}
