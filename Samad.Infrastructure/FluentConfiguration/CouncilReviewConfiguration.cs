using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Samad.Domain.Entity;

namespace Samad.Infrastructure.FluentConfiguration
{
    public class CouncilReviewConfiguration : IEntityTypeConfiguration<CouncilReview>
    {
        public void Configure(EntityTypeBuilder<CouncilReview> builder)
        {
            builder.HasKey(cr => cr.Id);
            builder.Property(cr => cr.Comment).HasMaxLength(500);

            builder.HasOne(cr => cr.Request)
                   .WithMany(r => r.CouncilReviews)
                   .HasForeignKey(cr => cr.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cr => cr.CouncilMember)
                   .WithMany(u => u.MyReviews)
                   .HasForeignKey(cr => cr.CouncilMemberId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
