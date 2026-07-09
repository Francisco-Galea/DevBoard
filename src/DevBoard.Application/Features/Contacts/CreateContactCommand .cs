using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using MediatR;

namespace DevBoard.Application.Features.Contacts;

public record CreateContactCommand(
    Guid UserId,
    string FullName,
    string? Company,
    string? Role,
    string? Email,
    string? LinkedInUrl,
    string? Notes
) : IRequest<Result<ContactDto>>;

public class CreateContactCommandHandler
    : IRequestHandler<CreateContactCommand, Result<ContactDto>>
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContactCommandHandler(
        IContactRepository contactRepository,
        IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ContactDto>> Handle(
        CreateContactCommand command,
        CancellationToken cancellationToken)
    {
        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            FullName = command.FullName.Trim(),
            Company = command.Company?.Trim(),
            Role = command.Role?.Trim(),
            Email = command.Email?.Trim(),
            LinkedInUrl = command.LinkedInUrl?.Trim(),
            Notes = command.Notes?.Trim()
        };

        await _contactRepository.AddAsync(contact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ContactDto>.Success(MapToDto(contact));
    }

    private static ContactDto MapToDto(Contact c) => new(
        c.Id, c.FullName, c.Company, c.Role,
        c.Email, c.LinkedInUrl, c.Notes, c.CreatedAt
    );
}