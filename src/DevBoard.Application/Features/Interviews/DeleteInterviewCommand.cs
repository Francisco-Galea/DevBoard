using DevBoard.Application.Common;
using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.Interviews;

public record DeleteInterviewCommand(Guid Id, Guid UserId) : IRequest<Result>;

public class DeleteInterviewCommandHandler
    : IRequestHandler<DeleteInterviewCommand, Result>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInterviewCommandHandler(
        IInterviewRepository interviewRepository,
        IJobApplicationRepository jobApplicationRepository,
        IUnitOfWork unitOfWork)
    {
        _interviewRepository = interviewRepository;
        _jobApplicationRepository = jobApplicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteInterviewCommand command,
        CancellationToken cancellationToken)
    {
        var interview = await _interviewRepository
            .GetByIdAsync(command.Id, cancellationToken);

        if (interview is null)
            return Result.Failure("Entrevista no encontrada.");

        var jobApplication = await _jobApplicationRepository
            .GetByIdAsync(interview.JobApplicationId, cancellationToken);

        if (jobApplication is null || jobApplication.UserId != command.UserId)
            return Result.Failure("No tenés permiso para eliminar esta entrevista.");

        _interviewRepository.Delete(interview);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}