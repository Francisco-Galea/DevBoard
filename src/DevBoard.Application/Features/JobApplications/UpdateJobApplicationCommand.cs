using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.JobApplications;

public record UpdateJobApplicationCommand(
    Guid Id,
    Guid UserId,
    string CompanyName,
    string Position,
    string? JobUrl,
    string? Notes,
    DateTime AppliedAt,
    Guid? ContactId
) : IRequest<Result<JobApplicationDto>>;

public class UpdateJobApplicationCommandHandler
    : IRequestHandler<UpdateJobApplicationCommand, Result<JobApplicationDto>>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IContactRepository _contactRepository;


    public UpdateJobApplicationCommandHandler(
        IJobApplicationRepository jobApplicationRepository,
        IUnitOfWork unitOfWork,
        IContactRepository contactRepository)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _unitOfWork = unitOfWork;
        _contactRepository = contactRepository;
    }

    public async Task<Result<JobApplicationDto>> Handle(
        UpdateJobApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var jobApplication = await _jobApplicationRepository
        .GetByIdAsync(command.Id, cancellationToken);

        if (jobApplication is null)
            return Result<JobApplicationDto>.Failure("Postulación no encontrada.");

        if (jobApplication.UserId != command.UserId)
            return Result<JobApplicationDto>.Failure("No tenés permiso para modificar esta postulación.");

        string? contactName = null;
        if (command.ContactId.HasValue)
        {
            var contact = await _contactRepository
                .GetByIdAsync(command.ContactId.Value, cancellationToken);
            contactName = contact?.FullName;
        }

        jobApplication.CompanyName = command.CompanyName.Trim();
        jobApplication.Position = command.Position.Trim();
        jobApplication.JobUrl = command.JobUrl?.Trim();
        jobApplication.Notes = command.Notes?.Trim();
        jobApplication.AppliedAt = command.AppliedAt;
        jobApplication.ContactId = command.ContactId;
        jobApplication.ContactName = contactName;

        _jobApplicationRepository.Update(jobApplication);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<JobApplicationDto>.Success(new JobApplicationDto(
            jobApplication.Id,
            jobApplication.CompanyName,
            jobApplication.Position,
            jobApplication.JobUrl,
            jobApplication.Notes,
            jobApplication.CurrentStatus,
            jobApplication.AppliedAt,
            jobApplication.CreatedAt,
            jobApplication.ContactId,
            jobApplication.ContactName
        ));
    }
}