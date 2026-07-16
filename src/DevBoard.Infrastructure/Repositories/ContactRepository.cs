using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using DevBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Contacts.FindAsync([id], cancellationToken);

        public async Task<IEnumerable<Contact>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _context.Contacts
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.FullName)
                .ToListAsync(cancellationToken);

        public async Task AddAsync(Contact contact, CancellationToken cancellationToken = default)
            => await _context.Contacts.AddAsync(contact, cancellationToken);

        public void Update(Contact contact)
            => _context.Contacts.Update(contact);

        public void Delete(Contact contact)
        {
            contact.IsDeleted = true;
            _context.Contacts.Update(contact);
        }
    }
}
