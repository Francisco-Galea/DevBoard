using DevBoard.Application.Common;
using DevBoard.Application.DTOs;
using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.Auth;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(
            command.RefreshToken,
            cancellationToken);

        if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<AuthResponse>.Failure("Refresh token inválido o expirado.");

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);

        return Result<AuthResponse>.Success(new AuthResponse(
            accessToken,
            newRefreshToken,
            user.Email,
            user.FirstName
        ));
    }
}