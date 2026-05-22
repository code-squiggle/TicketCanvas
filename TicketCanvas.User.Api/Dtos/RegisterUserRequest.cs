namespace TicketCanvas.User.Api.Dtos;

public record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName);