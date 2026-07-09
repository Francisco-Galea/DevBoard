using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.JobApplications;

public record GetJobApplicationsQuery(Guid UserId)
    : IRequest<Result<IEnumerable<JobApplicationDto>>>;

public class GetJobApplicationsQueryHandler
    : IRequestHandler<GetJobApplicationsQuery, Result<IEnumerable<JobApplicationDto>>>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;

    public GetJobApplicationsQueryHandler(IJobApplicationRepository jobApplicationRepository)
    {
        _jobApplicationRepository = jobApplicationRepository;
    }

    public async Task<Result<IEnumerable<JobApplicationDto>>> Handle(
        GetJobApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var jobApplications = await _jobApplicationRepository
            .GetByUserIdAsync(request.UserId, cancellationToken);

        var dtos = jobApplications.Select(j => new JobApplicationDto(
            j.Id, j.CompanyName, j.Position, j.JobUrl,
            j.Notes, j.CurrentStatus, j.AppliedAt, j.CreatedAt,
            j.ContactId, j.Contact?.FullName
        ));

        return Result<IEnumerable<JobApplicationDto>>.Success(dtos);
    }
}