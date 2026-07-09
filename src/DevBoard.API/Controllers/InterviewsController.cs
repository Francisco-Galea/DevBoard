using DevBoard.Application.DTOs;
using DevBoard.Application.Features.Interviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InterviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet("job-application/{jobApplicationId:guid}")]
    public async Task<IActionResult> GetByJobApplication(
        Guid jobApplicationId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetInterviewsByJobApplicationQuery(jobApplicationId, GetUserId()),
            cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { success = false, error = result.Error });

        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateInterviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateInterviewCommand(
            GetUserId(),
            request.JobApplicationId,
            request.Type,
            request.Notes,
            request.ScheduledAt);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, error = result.Error });

        return Ok(new { success = true, data = result.Value });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateInterviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateInterviewCommand(
            id,
            GetUserId(),
            request.Type,
            request.Notes,
            request.ScheduledAt);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { success = false, error = result.Error });

        return Ok(new { success = true, data = result.Value });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteInterviewCommand(id, GetUserId()), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { success = false, error = result.Error });

        return NoContent();
    }
}