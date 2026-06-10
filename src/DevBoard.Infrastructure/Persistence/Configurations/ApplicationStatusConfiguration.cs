using DevBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBoard.Infrastructure.Persistence.Configurations
{
    public class ApplicationStatusConfiguration : IEntityTypeConfiguration<ApplicationStatus>
    {
        public void Configure(EntityTypeBuilder<ApplicationStatus> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Notes)
                .HasMaxLength(1000);
        }
    }
}
