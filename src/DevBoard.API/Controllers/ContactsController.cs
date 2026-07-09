using DevBoard.Application.DTOs;
using DevBoard.Application.Features.Contacts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContactsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactsController(IMediator mediator)
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
            new GetContactsQuery(GetUserId()), cancellationToken);

        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetContactByIdQuery(id, GetUserId()), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { success = false, error = result.Error });

        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateContactRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateContactCommand(
            GetUserId(),
            request.FullName,
            request.Company,
            request.Role,
            request.Email,
            request.LinkedInUrl,
            request.Notes);

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
        UpdateContactRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateContactCommand(
            id,
            GetUserId(),
            request.FullName,
            request.Company,
            request.Role,
            request.Email,
            request.LinkedInUrl,
            request.Notes);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { success = false, error = result.Error });

        return Ok(new { success = true, data = result.Value });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteContactCommand(id, GetUserId()), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { success = false, error = result.Error });

        return NoContent();
    }
}