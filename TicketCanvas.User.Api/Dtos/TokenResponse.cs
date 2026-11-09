namespace TicketCanvas.User.Api.Dtos;

public record TokenResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);