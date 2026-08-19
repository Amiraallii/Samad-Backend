using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Samad.Domain.Entity;

namespace Samad.Infrastructure.FluentConfiguration
{
    public class RequestCouncilAssignmentConfiguration
    : IEntityTypeConfiguration<RequestCouncilAssignment>
    {
        public void Configure(
            EntityTypeBuilder<RequestCouncilAssignment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.RequestId,
                x.CouncilMemberId
            })
            .IsUnique();

            builder
                .HasOne(x => x.Request)
                .WithMany(x => x.CouncilAssignments)
                .HasForeignKey(x => x.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.CouncilMember)
                .WithMany()
                .HasForeignKey(x => x.CouncilMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
