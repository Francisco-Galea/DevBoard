using DevBoard.Domain.Entities;

namespace DevBoard.Application.Interfaces.Repositories
{
    public interface IContactRepository
    {
        Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Contact>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(Contact contact, CancellationToken cancellationToken = default);
        void Update(Contact contact);
        void Delete(Contact contact);
    }
}
