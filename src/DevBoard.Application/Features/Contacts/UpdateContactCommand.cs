using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.Contacts;

public record UpdateContactCommand(
    Guid Id,
    Guid UserId,
    string FullName,
    string? Company,
    string? Role,
    string? Email,
    string? LinkedInUrl,
    string? Notes
) : IRequest<Result<ContactDto>>;

public class UpdateContactCommandHandler
    : IRequestHandler<UpdateContactCommand, Result<ContactDto>>
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContactCommandHandler(
        IContactRepository contactRepository,
        IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ContactDto>> Handle(
        UpdateContactCommand command,
        CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(command.Id, cancellationToken);

        if (contact is null)
            return Result<ContactDto>.Failure("Contacto no encontrado.");

        if (contact.UserId != command.UserId)
            return Result<ContactDto>.Failure("No tenés permiso para modificar este contacto.");

        contact.FullName = command.FullName.Trim();
        contact.Company = command.Company?.Trim();
        contact.Role = command.Role?.Trim();
        contact.Email = command.Email?.Trim();
        contact.LinkedInUrl = command.LinkedInUrl?.Trim();
        contact.Notes = command.Notes?.Trim();

        _contactRepository.Update(contact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ContactDto>.Success(new ContactDto(
            contact.Id, contact.FullName, contact.Company, contact.Role,
            contact.Email, contact.LinkedInUrl, contact.Notes, contact.CreatedAt
        ));
    }
}