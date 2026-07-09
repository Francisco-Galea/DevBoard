namespace DevBoard.Application.DTOs;

public record CreateContactRequest(
    string FullName,
    string? Company,
    string? Role,
    string? Email,
    string? LinkedInUrl,
    string? Notes
);

public record UpdateContactRequest(
    string FullName,
    string? Company,
    string? Role,
    string? Email,
    string? LinkedInUrl,
    string? Notes
);

public record ContactDto(
    Guid Id,
    string FullName,
    string? Company,
    string? Role,
    string? Email,
    string? LinkedInUrl,
    string? Notes,
    DateTime CreatedAt
);