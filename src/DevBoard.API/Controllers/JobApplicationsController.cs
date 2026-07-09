using DevBoard.Application.DTOs;
using DevBoard.Application.Features.JobApplications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetJobApplicationsQuery(GetUserId()), cancellationToken);

        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetJobApplicationByIdQuery(id, GetUserId()), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { success = false, error = result.Error });

        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateJobApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateJobApplicationCommand(
            GetUserId(),
            request.CompanyName,
            request.Position,
            request.JobUrl,
            request.Notes,
            request.AppliedAt,
            request.ContactId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, error = result.Error });

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            new { success = true, data = result.Value });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateJobApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateJobApplicationCommand(
            id,
            GetUserId(),
            request.CompanyName,
            request.Position,
            request.JobUrl,
            request.Notes,
            request.AppliedAt,
            request.ContactId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { success = false, error = result.Error });

        return Ok(new { success = true, data = result.Value });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteJobApplicationCommand(id, GetUserId()), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { success = false, error = result.Error });

        return NoContent();
    }

    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        ChangeStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeJobApplicationStatusCommand(
            id,
            GetUserId(),
            request.NewStatus,
            request.Notes);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, error = result.Error });

        return Ok(new { success = true, data = result.Value });
    }
}