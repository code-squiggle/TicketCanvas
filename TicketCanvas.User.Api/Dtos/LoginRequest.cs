namespace TicketCanvas.User.Api.Dtos;

public record LoginRequest(
    string Email,
    string Password);