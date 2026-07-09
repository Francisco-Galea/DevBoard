using DevBoard.Domain.Entities;

namespace DevBoard.Application.Interfaces.Repositories;

public interface IApplicationStatusRepository
{
    Task AddAsync(ApplicationStatus status, CancellationToken cancellationToken = default);
}