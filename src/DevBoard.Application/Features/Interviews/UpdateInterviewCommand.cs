using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.Interviews;

public record UpdateInterviewCommand(
    Guid Id,
    Guid UserId,
    string Type,
    string? Notes,
    DateTime ScheduledAt
) : IRequest<Result<InterviewDto>>;

public class UpdateInterviewCommandHandler
    : IRequestHandler<UpdateInterviewCommand, Result<InterviewDto>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInterviewCommandHandler(
        IInterviewRepository interviewRepository,
        IJobApplicationRepository jobApplicationRepository,
        IUnitOfWork unitOfWork)
    {
        _interviewRepository = interviewRepository;
        _jobApplicationRepository = jobApplicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<InterviewDto>> Handle(
        UpdateInterviewCommand command,
        CancellationToken cancellationToken)
    {
        var interview = await _interviewRepository
            .GetByIdAsync(command.Id, cancellationToken);

        if (interview is null)
            return Result<InterviewDto>.Failure("Entrevista no encontrada.");

        var jobApplication = await _jobApplicationRepository
            .GetByIdAsync(interview.JobApplicationId, cancellationToken);

        if (jobApplication is null || jobApplication.UserId != command.UserId)
            return Result<InterviewDto>.Failure("No tenés permiso para modificar esta entrevista.");

        interview.Type = command.Type.Trim();
        interview.Notes = command.Notes?.Trim();
        interview.ScheduledAt = command.ScheduledAt;

        _interviewRepository.Update(interview);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<InterviewDto>.Success(new InterviewDto(
            interview.Id,
            interview.Type,
            interview.Notes,
            interview.ScheduledAt
        ));
    }
}