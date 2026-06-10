using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using DevBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Users.FindAsync([id], cancellationToken);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
            => await _context.Users
                .AnyAsync(u => u.Email == email, cancellationToken);

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
            => await _context.Users.AddAsync(user, cancellationToken);

        public void Update(User user)
            => _context.Users.Update(user);

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
            => await _context.Users
        .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
    }
}
