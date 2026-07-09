using DevBoard.Application.Common;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.JobApplications;

public record DeleteJobApplicationCommand(
    Guid Id,
    Guid UserId
) : IRequest<Result>;

public class DeleteJobApplicationCommandHandler
    : IRequestHandler<DeleteJobApplicationCommand, Result>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteJobApplicationCommandHandler(
        IJobApplicationRepository jobApplicationRepository,
        IUnitOfWork unitOfWork)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteJobApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var jobApplication = await _jobApplicationRepository
            .GetByIdAsync(command.Id, cancellationToken);

        if (jobApplication is null)
            return Result.Failure("Postulación no encontrada.");

        if (jobApplication.UserId != command.UserId)
            return Result.Failure("No tenés permiso para eliminar esta postulación.");

        _jobApplicationRepository.Delete(jobApplication);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}