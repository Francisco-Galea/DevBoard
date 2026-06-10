using DevBoard.Application.DTOs;
using DevBoard.Application.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { success = false, error = result.Error });

        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new { success = false, error = result.Error });

        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new { success = false, error = result.Error });

        return Ok(new { success = true, data = result.Value });
    }
}