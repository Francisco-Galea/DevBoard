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
        Guid? ContactId,
        string? ContactFullName
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
        Guid? ContactId,
        string? ContactFullName,
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
