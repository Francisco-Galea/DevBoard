using DevBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBoard.Infrastructure.Persistence.Configurations
{
    public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Type)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Notes)
                .HasMaxLength(1000);
        }
    }
}
