using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Samad.Domain.Entity;

namespace Samad.Infrastructure.FluentConfiguration
{
    public class CouncilMemberConfiguration
        : IEntityTypeConfiguration<CouncilMember>
    {
        public void Configure(
            EntityTypeBuilder<CouncilMember> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.HasOne(x => x.User)
                .WithOne(x => x.CouncilMembership)
                .HasForeignKey<CouncilMember>(
                    x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}