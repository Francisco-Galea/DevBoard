using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using MediatR;

namespace DevBoard.Application.Features.Interviews;

public record CreateInterviewCommand(
    Guid UserId,
    Guid JobApplicationId,
    string Type,
    string? Notes,
    DateTime ScheduledAt
) : IRequest<Result<InterviewDto>>;

public class CreateInterviewCommandHandler
    : IRequestHandler<CreateInterviewCommand, Result<InterviewDto>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInterviewCommandHandler(
        IInterviewRepository interviewRepository,
        IJobApplicationRepository jobApplicationRepository,
        IUnitOfWork unitOfWork)
    {
        _interviewRepository = interviewRepository;
        _jobApplicationRepository = jobApplicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<InterviewDto>> Handle(
        CreateInterviewCommand command,
        CancellationToken cancellationToken)
    {
        var jobApplication = await _jobApplicationRepository
            .GetByIdAsync(command.JobApplicationId, cancellationToken);

        if (jobApplication is null)
            return Result<InterviewDto>.Failure("Postulación no encontrada.");

        if (jobApplication.UserId != command.UserId)
            return Result<InterviewDto>.Failure("No tenés permiso para agregar entrevistas a esta postulación.");

        var interview = new Interview
        {
            Id = Guid.NewGuid(),
            JobApplicationId = command.JobApplicationId,
            Type = command.Type.Trim(),
            Notes = command.Notes?.Trim(),
            ScheduledAt = command.ScheduledAt
        };

        await _interviewRepository.AddAsync(interview, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<InterviewDto>.Success(new InterviewDto(
            interview.Id,
            interview.Type,
            interview.Notes,
            interview.ScheduledAt
        ));
    }
}