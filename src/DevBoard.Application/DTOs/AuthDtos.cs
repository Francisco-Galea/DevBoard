namespace DevBoard.Application.DTOs
{
    public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);

    public record LoginRequest(
        string Email,
        string Password
    );

    public record AuthResponse(
        string AccessToken,
        string RefreshToken,
        string Email,
        string FirstName
    );

    public record RefreshTokenRequest(
        string RefreshToken
    );
}
