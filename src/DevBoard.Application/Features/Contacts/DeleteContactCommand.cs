using DevBoard.Application.Common;
using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.Contacts;

public record DeleteContactCommand(Guid Id, Guid UserId) : IRequest<Result>;

public class DeleteContactCommandHandler
    : IRequestHandler<DeleteContactCommand, Result>
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContactCommandHandler(
        IContactRepository contactRepository,
        IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteContactCommand command,
        CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(command.Id, cancellationToken);

        if (contact is null)
            return Result.Failure("Contacto no encontrado.");

        if (contact.UserId != command.UserId)
            return Result.Failure("No tenés permiso para eliminar este contacto.");

        _contactRepository.Delete(contact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}