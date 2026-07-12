using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using DevBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Repositories
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly ApplicationDbContext _context;

        public InterviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Interview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Interviews.FindAsync([id], cancellationToken);

        public async Task<IEnumerable<Interview>> GetByJobApplicationIdAsync(Guid jobApplicationId, CancellationToken cancellationToken = default)
            => await _context.Interviews
                .Where(i => i.JobApplicationId == jobApplicationId)
                .OrderBy(i => i.ScheduledAt)
                .ToListAsync(cancellationToken);

        public async Task AddAsync(Interview interview, CancellationToken cancellationToken = default)
            => await _context.Interviews.AddAsync(interview, cancellationToken);

        public void Update(Interview interview)
            => _context.Interviews.Update(interview);

        public void Delete(Interview interview)
            => _context.Interviews.Remove(interview);

        public async Task<IEnumerable<Interview>> GetUpcomingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _context.Interviews
                .Include(i => i.JobApplication)
                .Where(i => i.JobApplication.UserId == userId
                && i.ScheduledAt >= DateTime.UtcNow)
                .OrderBy(i => i.ScheduledAt)
                .Take(5)
                .ToListAsync(cancellationToken);
    }
}
