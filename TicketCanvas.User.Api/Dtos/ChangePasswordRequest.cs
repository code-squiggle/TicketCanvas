namespace TicketCanvas.User.Api.Dtos;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);