using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.Dashboard;

public record GetDashboardQuery(Guid UserId) : IRequest<Result<DashboardDto>>;

public class GetDashboardQueryHandler
    : IRequestHandler<GetDashboardQuery, Result<DashboardDto>>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IInterviewRepository _interviewRepository;

    public GetDashboardQueryHandler(
        IJobApplicationRepository jobApplicationRepository,
        IInterviewRepository interviewRepository)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _interviewRepository = interviewRepository;
    }

    public async Task<Result<DashboardDto>> Handle(
        GetDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var total = await _jobApplicationRepository
            .CountByUserIdAsync(request.UserId, cancellationToken);

        var thisMonth = await _jobApplicationRepository
            .CountByUserIdThisMonthAsync(request.UserId, cancellationToken);

        var thisMonthApplications = await _jobApplicationRepository
            .GetThisMonthByUserIdAsync(request.UserId, cancellationToken);

        var statusBreakdown = thisMonthApplications
            .GroupBy(j => j.CurrentStatus)
            .Select(g => new StatusBreakdownDto(g.Key, g.Count()));

        var staleApplications = await _jobApplicationRepository
            .GetStaleByUserIdAsync(request.UserId, staleDays: 14, cancellationToken);

        var staleDto = staleApplications.Select(j => new StaleApplicationDto(
            j.Id,
            j.CompanyName,
            j.Position,
            j.CurrentStatus,
            (int)(DateTime.UtcNow - j.UpdatedAt).TotalDays
        ));

        var lastApplication = await _jobApplicationRepository
            .GetLastByUserIdAsync(request.UserId, cancellationToken);

        var lastDto = lastApplication is null ? null : new JobApplicationDto(
            lastApplication.Id,
            lastApplication.CompanyName,
            lastApplication.Position,
            lastApplication.JobUrl,
            lastApplication.Notes,
            lastApplication.CurrentStatus,
            lastApplication.AppliedAt,
            lastApplication.CreatedAt,
            lastApplication.ContactId,
            null
        );

        var upcomingInterviews = await _interviewRepository
            .GetUpcomingByUserIdAsync(request.UserId, cancellationToken);

        var upcomingDto = upcomingInterviews.Select(i => new UpcomingInterviewDto(
            i.Id,
            i.JobApplication.CompanyName,
            i.JobApplication.Position,
            i.Type,
            i.ScheduledAt
        ));

        return Result<DashboardDto>.Success(new DashboardDto(
            total,
            thisMonth,
            statusBreakdown,
            staleDto,
            lastDto,
            upcomingDto
        ));
    }
}