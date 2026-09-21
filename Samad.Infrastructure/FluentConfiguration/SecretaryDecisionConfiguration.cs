using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Samad.Domain.Entity;

namespace Samad.Infrastructure.FluentConfiguration
{
    public class SecretaryDecisionConfiguration
        : IEntityTypeConfiguration<SecretaryDecision>
    {
        public void Configure(
            EntityTypeBuilder<SecretaryDecision> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Stage)
                .IsRequired();

            builder.Property(x => x.Decision)
                .IsRequired();

            builder.Property(x => x.Comment)
                .HasMaxLength(1000);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x =>
                new
                {
                    x.RequestId,
                    x.Stage,
                    x.CreatedAt
                });

            builder.HasOne(x => x.Request)
                .WithMany(x => x.SecretaryDecisions)
                .HasForeignKey(x => x.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Secretary)
                .WithMany()
                .HasForeignKey(x => x.SecretaryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}