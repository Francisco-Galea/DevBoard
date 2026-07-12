using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using DevBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public JobApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<JobApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.JobApplications
                .FindAsync([id], cancellationToken);

        public async Task<JobApplication?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.JobApplications
                .Include(j => j.StatusHistory.OrderByDescending(s => s.ChangedAt))
                .Include(j => j.Interviews.OrderBy(i => i.ScheduledAt))
                .Include(j => j.Contact)
                .FirstOrDefaultAsync(j => j.Id== id, cancellationToken);

        public async Task<IEnumerable<JobApplication>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _context.JobApplications
                .Where(j => j.UserId == userId)
                .OrderByDescending(j => j.AppliedAt)
                .ToListAsync(cancellationToken);

        public async Task AddAsync(JobApplication jobApplication, CancellationToken cancellationToken = default)
            => await _context.JobApplications.AddAsync(jobApplication, cancellationToken);

        public void Update(JobApplication jobApplication)
            => _context.JobApplications.Update(jobApplication);

        public void Delete(JobApplication jobApplication)
            => _context.JobApplications.Remove(jobApplication);

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
            => await _context.Users
               .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);

        public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    => await _context.JobApplications
        .CountAsync(j => j.UserId == userId, cancellationToken);

        public async Task<int> CountByUserIdThisMonthAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            return await _context.JobApplications
                .CountAsync(j => j.UserId == userId && j.AppliedAt >= firstDayOfMonth, cancellationToken);
        }

        public async Task<IEnumerable<JobApplication>> GetThisMonthByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            return await _context.JobApplications
                .Where(j => j.UserId == userId && j.AppliedAt >= firstDayOfMonth)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<JobApplication>> GetStaleByUserIdAsync(Guid userId, int staleDays, CancellationToken cancellationToken = default)
        {
            var threshold = DateTime.UtcNow.AddDays(-staleDays);
            return await _context.JobApplications
                .Where(j => j.UserId == userId
                    && j.UpdatedAt < threshold
                    && j.CurrentStatus != "Rejected"
                    && j.CurrentStatus != "Withdrawn")
                .OrderBy(j => j.UpdatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<JobApplication?> GetLastByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _context.JobApplications
                .Where(j => j.UserId == userId)
                .OrderByDescending(j => j.AppliedAt)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
