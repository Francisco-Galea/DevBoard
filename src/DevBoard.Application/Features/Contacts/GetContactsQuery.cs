using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.Contacts;

public record GetContactsQuery(Guid UserId)
    : IRequest<Result<IEnumerable<ContactDto>>>;

public class GetContactsQueryHandler
    : IRequestHandler<GetContactsQuery, Result<IEnumerable<ContactDto>>>
{
    private readonly IContactRepository _contactRepository;

    public GetContactsQueryHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<Result<IEnumerable<ContactDto>>> Handle(
        GetContactsQuery request,
        CancellationToken cancellationToken)
    {
        var contacts = await _contactRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var dtos = contacts.Select(c => new ContactDto(
            c.Id, c.FullName, c.Company, c.Role,
            c.Email, c.LinkedInUrl, c.Notes, c.CreatedAt
        ));

        return Result<IEnumerable<ContactDto>>.Success(dtos);
    }
}