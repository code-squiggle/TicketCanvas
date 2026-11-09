namespace TicketCanvas.User.Api.Dtos;

public record RefreshResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);