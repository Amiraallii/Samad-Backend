using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Samad.Domain.Entity;

namespace Samad.Infrastructure.FluentConfiguration
{
    public class RequestConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Title).IsRequired().HasMaxLength(200);
            builder.Property(r => r.Description).HasMaxLength(1000);
            builder.Property(r => r.FinalSecretaryComment).HasMaxLength(500);

            builder.HasOne(r => r.Applicant)
                   .WithMany(u => u.MyRequests)
                   .HasForeignKey(r => r.ApplicantId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Secretary)
                   .WithMany() 
                   .HasForeignKey(r => r.SecretaryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
