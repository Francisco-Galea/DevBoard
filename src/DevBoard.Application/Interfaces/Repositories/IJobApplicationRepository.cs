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
    }
}
