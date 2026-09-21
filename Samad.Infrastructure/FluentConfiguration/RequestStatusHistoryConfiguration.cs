using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Samad.Domain.Entity;

namespace Samad.Infrastructure.FluentConfiguration
{
    public class RequestStatusHistoryConfiguration
        : IEntityTypeConfiguration<RequestStatusHistory>
    {
        public void Configure(
            EntityTypeBuilder<RequestStatusHistory> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FromStatus)
                .IsRequired(false);

            builder.Property(x => x.ToStatus)
                .IsRequired();

            builder.Property(x => x.ChangedAt)
                .IsRequired();

            builder.Property(x => x.Comment)
                .HasMaxLength(1000);

            builder.HasIndex(x => new
            {
                x.RequestId,
                x.ChangedAt
            });

            builder
                .HasOne(x => x.Request)
                .WithMany(x => x.StatusHistory)
                .HasForeignKey(x => x.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.ChangedByUser)
                .WithMany()
                .HasForeignKey(x => x.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}