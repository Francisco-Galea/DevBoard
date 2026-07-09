namespace DevBoard.Application.DTOs;

public record CreateInterviewRequest(
    Guid JobApplicationId,
    string Type,
    string? Notes,
    DateTime ScheduledAt
);

public record UpdateInterviewRequest(
    string Type,
    string? Notes,
    DateTime ScheduledAt
);