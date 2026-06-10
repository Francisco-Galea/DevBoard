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
    }
}
