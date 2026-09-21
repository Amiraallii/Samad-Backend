using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Samad.Domain.Entity;
using Samad.Domain.Enum;

namespace Samad.Infrastructure.FluentConfiguration
{
    public class RequestDocumentConfiguration : IEntityTypeConfiguration<RequestDocument>
    {
        public void Configure(EntityTypeBuilder<RequestDocument> builder)
        {
            builder.HasKey(rd => rd.Id);
            builder.Property(rd => rd.FileUrl)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(rd => rd.DocumentType)
               .IsRequired()
               .HasDefaultValue(DocumentType.Other);
            builder.HasOne(rd => rd.Request)
                   .WithMany(r => r.Documents)
                   .HasForeignKey(rd => rd.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
