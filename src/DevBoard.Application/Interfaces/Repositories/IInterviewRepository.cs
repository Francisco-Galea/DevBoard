using DevBoard.Domain.Entities;

namespace DevBoard.Application.Interfaces.Repositories
{
    public interface IInterviewRepository
    {
        Task<Interview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Interview>> GetByJobApplicationIdAsync(Guid jobApplicationId, CancellationToken cancellationToken = default);
        Task AddAsync(Interview interview, CancellationToken cancellationToken = default);
        void Update(Interview interview);
        void Delete(Interview interview);
        Task<IEnumerable<Interview>> GetUpcomingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
