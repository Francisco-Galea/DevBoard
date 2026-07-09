using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.Contacts;

public record GetContactByIdQuery(Guid Id, Guid UserId)
    : IRequest<Result<ContactDto>>;

public class GetContactByIdQueryHandler
    : IRequestHandler<GetContactByIdQuery, Result<ContactDto>>
{
    private readonly IContactRepository _contactRepository;

    public GetContactByIdQueryHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<Result<ContactDto>> Handle(
        GetContactByIdQuery request,
        CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.Id, cancellationToken);

        if (contact is null)
            return Result<ContactDto>.Failure("Contacto no encontrado.");

        if (contact.UserId != request.UserId)
            return Result<ContactDto>.Failure("No tenés permiso para ver este contacto.");

        return Result<ContactDto>.Success(new ContactDto(
            contact.Id, contact.FullName, contact.Company, contact.Role,
            contact.Email, contact.LinkedInUrl, contact.Notes, contact.CreatedAt
        ));
    }
}