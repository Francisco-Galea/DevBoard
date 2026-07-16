using DevBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBoard.Infrastructure.Persistence.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(j => j.Position)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(j => j.JobUrl)
            .HasMaxLength(500);

        builder.Property(j => j.CurrentStatus)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(j => j.StatusHistory)
            .WithOne(s => s.JobApplication)
            .HasForeignKey(s => s.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(j => j.Interviews)
            .WithOne(i => i.JobApplication)
            .HasForeignKey(i => i.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(j => j.Contact)
            .WithMany(c => c.JobApplications)
            .HasForeignKey(j => j.ContactId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(j => j.ContactName)
            .HasMaxLength(200);
    }
}