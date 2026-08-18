namespace DevBoard.Application.DTOs
{
    public record CreateJobApplicationRequest(
        string CompanyName,
        string Position,
        string? JobUrl,
        string? Notes,
        DateTime AppliedAt,
        Guid? ContactId
    );

    public record UpdateJobApplicationRequest(
        string CompanyName,
        string Position,
        string? JobUrl,
        string? Notes,
        DateTime AppliedAt,
        Guid? ContactId
    );

    public record JobApplicationDto(
        Guid Id,
        string CompanyName,
        string Position,
        string? JobUrl,
        string? Notes,
        string CurrentStatus,
        DateTime AppliedAt,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        Guid? ContactId,
        string? ContactName
    );

    public record JobApplicationDetailDto(
        Guid Id,
        string CompanyName,
        string Position,
        string? JobUrl,
        string? Notes,
        string CurrentStatus,
        DateTime AppliedAt,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        Guid? ContactId,
        string? ContactName,
        IEnumerable<ApplicationStatusDto> StatusHistory,
        IEnumerable<InterviewDto> Interviews
    );

    public record ApplicationStatusDto(
        Guid Id,
        string Status,
        string? Notes,
        DateTime ChangedAt
    );

    public record InterviewDto(
        Guid Id,
        string Type,
        string? Notes,
        DateTime ScheduledAt
    );

    public record ChangeStatusRequest(string NewStatus, string? Notes);

}
