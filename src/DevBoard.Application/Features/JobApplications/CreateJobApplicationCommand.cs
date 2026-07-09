using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using MediatR;

namespace DevBoard.Application.Features.JobApplications;

public record CreateJobApplicationCommand(
    Guid UserId,
    string CompanyName,
    string Position,
    string? JobUrl,
    string? Notes,
    DateTime AppliedAt,
    Guid? ContactId
) : IRequest<Result<JobApplicationDto>>;

public class CreateJobApplicationCommandHandler
    : IRequestHandler<CreateJobApplicationCommand, Result<JobApplicationDto>>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateJobApplicationCommandHandler(
        IJobApplicationRepository jobApplicationRepository,
        IUnitOfWork unitOfWork)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<JobApplicationDto>> Handle(
        CreateJobApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var jobApplication = new JobApplication
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            CompanyName = command.CompanyName.Trim(),
            Position = command.Position.Trim(),
            JobUrl = command.JobUrl?.Trim(),
            Notes = command.Notes?.Trim(),
            CurrentStatus = "Applied",
            AppliedAt = command.AppliedAt,
            ContactId = command.ContactId
        };

        var initialStatus = new ApplicationStatus
        {
            Id = Guid.NewGuid(),
            JobApplicationId = jobApplication.Id,
            Status = "Applied",
            Notes = "Postulación creada.",
            ChangedAt = DateTime.UtcNow
        };

        jobApplication.StatusHistory.Add(initialStatus);

        await _jobApplicationRepository.AddAsync(jobApplication, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<JobApplicationDto>.Success(MapToDto(jobApplication));
    }

    private static JobApplicationDto MapToDto(JobApplication j) => new(
        j.Id, j.CompanyName, j.Position, j.JobUrl,
        j.Notes, j.CurrentStatus, j.AppliedAt, j.CreatedAt,
        j.ContactId, j.Contact?.FullName
    );
}