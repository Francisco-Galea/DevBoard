using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace DevBoard.Application.Features.Auth;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<Result<AuthResponse>>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
            return Result<AuthResponse>.Failure("El email ya está registrado.");

        var refreshToken = _tokenService.GenerateRefreshToken();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email.ToLower().Trim(),
            PasswordHash = _passwordHasher.Hash(command.Password),
            FirstName = command.FirstName.Trim(),
            LastName = command.LastName.Trim(),
            RefreshToken = refreshToken,
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);

        return Result<AuthResponse>.Success(new AuthResponse(
            accessToken,
            refreshToken,
            user.Email,
            user.FirstName
        ));
    }
}