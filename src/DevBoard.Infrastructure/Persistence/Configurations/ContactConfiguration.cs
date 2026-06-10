using DevBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBoard.Infrastructure.Persistence.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Company)
                .HasMaxLength(200);

            builder.Property(c => c.Role)
                .HasMaxLength(200);

            builder.Property(c => c.Email)
                .HasMaxLength(256);

            builder.Property(c => c.LinkedInUrl)
                .HasMaxLength(500);

            builder.Property(c => c.Notes)
                .HasMaxLength(1000);
        }
    }
}
