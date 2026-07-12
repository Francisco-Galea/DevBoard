namespace DevBoard.Application.DTOs;

public record DashboardDto(
    int TotalApplications,
    int ApplicationsThisMonth,
    IEnumerable<StatusBreakdownDto> StatusBreakdownThisMonth,
    IEnumerable<StaleApplicationDto> StaleApplications,
    JobApplicationDto? LastApplication,
    IEnumerable<UpcomingInterviewDto> UpcomingInterviews
);

public record StatusBreakdownDto(
    string Status,
    int Count
);

public record StaleApplicationDto(
    Guid Id,
    string CompanyName,
    string Position,
    string CurrentStatus,
    int DaysSinceLastUpdate
);

public record UpcomingInterviewDto(
    Guid Id,
    string CompanyName,
    string Position,
    string Type,
    DateTime ScheduledAt
);