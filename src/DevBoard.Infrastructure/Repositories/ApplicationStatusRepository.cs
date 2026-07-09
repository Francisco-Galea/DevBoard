using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using DevBoard.Infrastructure.Persistence;

namespace DevBoard.Infrastructure.Repositories;

public class ApplicationStatusRepository : IApplicationStatusRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationStatusRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ApplicationStatus status, CancellationToken cancellationToken = default)
        => await _context.ApplicationStatuses.AddAsync(status, cancellationToken);
}