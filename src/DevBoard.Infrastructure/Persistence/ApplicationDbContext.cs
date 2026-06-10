using DevBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DevBoard.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<ApplicationStatus> ApplicationStatuses => Set<ApplicationStatus>();
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Interview> Interviews => Set<Interview>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Property("UpdatedAt").CurrentValue is DateTime)
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                    if (entry.Property("CreatedAt").CurrentValue is DateTime dt && dt == default)
                        entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
