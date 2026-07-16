using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.JobApplications;

public record GetJobApplicationByIdQuery(Guid Id, Guid UserId)
    : IRequest<Result<JobApplicationDetailDto>>;

public class GetJobApplicationByIdQueryHandler
    : IRequestHandler<GetJobApplicationByIdQuery, Result<JobApplicationDetailDto>>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;

    public GetJobApplicationByIdQueryHandler(IJobApplicationRepository jobApplicationRepository)
    {
        _jobApplicationRepository = jobApplicationRepository;
    }

    public async Task<Result<JobApplicationDetailDto>> Handle(
        GetJobApplicationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var j = await _jobApplicationRepository
            .GetByIdWithDetailsAsync(request.Id, cancellationToken);

        if (j is null)
            return Result<JobApplicationDetailDto>.Failure("Postulación no encontrada.");

        if (j.UserId != request.UserId)
            return Result<JobApplicationDetailDto>.Failure("No tenés permiso para ver esta postulación.");

        var dto = new JobApplicationDetailDto(
            j.Id, j.CompanyName, j.Position, j.JobUrl,
            j.Notes, j.CurrentStatus, j.AppliedAt, j.CreatedAt,
            j.ContactId, j.ContactName,
            j.StatusHistory.Select(s => new ApplicationStatusDto(s.Id, s.Status, s.Notes, s.ChangedAt)),
            j.Interviews.Select(i => new InterviewDto(i.Id, i.Type, i.Notes, i.ScheduledAt))
        );

        return Result<JobApplicationDetailDto>.Success(dto);
    }
}