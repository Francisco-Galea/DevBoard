using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using MediatR;

namespace DevBoard.Application.Features.JobApplications;

public record ChangeJobApplicationStatusCommand(
    Guid Id,
    Guid UserId,
    string NewStatus,
    string? Notes
) : IRequest<Result<ApplicationStatusDto>>;

public class ChangeJobApplicationStatusCommandHandler
    : IRequestHandler<ChangeJobApplicationStatusCommand, Result<ApplicationStatusDto>>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IApplicationStatusRepository _applicationStatusRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeJobApplicationStatusCommandHandler(
        IJobApplicationRepository jobApplicationRepository,
        IApplicationStatusRepository applicationStatusRepository,
        IUnitOfWork unitOfWork)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _applicationStatusRepository = applicationStatusRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ApplicationStatusDto>> Handle(
        ChangeJobApplicationStatusCommand command,
        CancellationToken cancellationToken)
    {
        var jobApplication = await _jobApplicationRepository
            .GetByIdAsync(command.Id, cancellationToken);

        if (jobApplication is null)
            return Result<ApplicationStatusDto>.Failure("Postulación no encontrada.");

        if (jobApplication.UserId != command.UserId)
            return Result<ApplicationStatusDto>.Failure("No tenés permiso para modificar esta postulación.");

        var validStatuses = new[]
        {
            "Applied", "PhoneScreen", "TechnicalInterview",
            "Interview", "Offer", "Rejected", "Withdrawn"
        };

        if (!validStatuses.Contains(command.NewStatus))
            return Result<ApplicationStatusDto>.Failure("Estado inválido.");

        jobApplication.CurrentStatus = command.NewStatus;
        _jobApplicationRepository.Update(jobApplication);

        var statusEntry = new ApplicationStatus
        {
            Id = Guid.NewGuid(),
            JobApplicationId = jobApplication.Id,
            Status = command.NewStatus,
            Notes = command.Notes?.Trim(),
            ChangedAt = DateTime.UtcNow
        };

        await _applicationStatusRepository.AddAsync(statusEntry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ApplicationStatusDto>.Success(new ApplicationStatusDto(
            statusEntry.Id,
            statusEntry.Status,
            statusEntry.Notes,
            statusEntry.ChangedAt
        ));
    }
}