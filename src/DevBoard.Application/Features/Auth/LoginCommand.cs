using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Options;


namespace DevBoard.Application.Features.Auth;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(
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
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            command.Email.ToLower().Trim(),
            cancellationToken);

        if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Credenciales inválidas.");

        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        _userRepository.Update(user);
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