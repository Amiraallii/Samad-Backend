using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Samad.Domain.Entity;

namespace Samad.Infrastructure.FluentConfiguration
{
    public class CouncilSignatureConfiguration
        : IEntityTypeConfiguration<CouncilSignature>
    {
        public void Configure(EntityTypeBuilder<CouncilSignature> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IsSigned)
                .IsRequired();

            builder.Property(x => x.SignedAt)
                .IsRequired(false);

            builder.HasIndex(x =>
                new
                {
                    x.RequestId,
                    x.CouncilMemberId
                })
                .IsUnique();

            builder.HasOne(x => x.Request)
                .WithMany(x => x.CouncilSignatures)
                .HasForeignKey(x => x.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CouncilMember)
                .WithMany()
                .HasForeignKey(x => x.CouncilMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}