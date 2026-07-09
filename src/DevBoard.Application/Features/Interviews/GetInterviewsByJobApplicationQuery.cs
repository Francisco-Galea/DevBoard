using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.Interviews;

public record GetInterviewsByJobApplicationQuery(Guid JobApplicationId, Guid UserId)
    : IRequest<Result<IEnumerable<InterviewDto>>>;

public class GetInterviewsByJobApplicationQueryHandler
    : IRequestHandler<GetInterviewsByJobApplicationQuery, Result<IEnumerable<InterviewDto>>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly IJobApplicationRepository _jobApplicationRepository;

    public GetInterviewsByJobApplicationQueryHandler(
        IInterviewRepository interviewRepository,
        IJobApplicationRepository jobApplicationRepository)
    {
        _interviewRepository = interviewRepository;
        _jobApplicationRepository = jobApplicationRepository;
    }

    public async Task<Result<IEnumerable<InterviewDto>>> Handle(
        GetInterviewsByJobApplicationQuery request,
        CancellationToken cancellationToken)
    {
        var jobApplication = await _jobApplicationRepository
            .GetByIdAsync(request.JobApplicationId, cancellationToken);

        if (jobApplication is null)
            return Result<IEnumerable<InterviewDto>>.Failure("Postulación no encontrada.");

        if (jobApplication.UserId != request.UserId)
            return Result<IEnumerable<InterviewDto>>.Failure("No tenés permiso para ver estas entrevistas.");

        var interviews = await _interviewRepository
            .GetByJobApplicationIdAsync(request.JobApplicationId, cancellationToken);

        var dtos = interviews.Select(i => new InterviewDto(
            i.Id, i.Type, i.Notes, i.ScheduledAt
        ));

        return Result<IEnumerable<InterviewDto>>.Success(dtos);
    }
}