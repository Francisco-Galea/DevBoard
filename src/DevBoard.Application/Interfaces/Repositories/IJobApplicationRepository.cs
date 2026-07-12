using DevBoard.Domain.Entities;

namespace DevBoard.Application.Interfaces.Repositories
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<JobApplication?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<JobApplication>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(JobApplication jobApplication, CancellationToken cancellationToken = default);
        void Update(JobApplication jobApplication);
        void Delete(JobApplication jobApplication);
        Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> CountByUserIdThisMonthAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<JobApplication>> GetThisMonthByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<JobApplication>> GetStaleByUserIdAsync(Guid userId, int staleDays, CancellationToken cancellationToken = default);
        Task<JobApplication?> GetLastByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
