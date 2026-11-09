using TicketCanvas.Common.Application;
using TicketCanvas.User.Data.Models;

namespace TicketCanvas.User.Api.Dtos;

public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    UserRole Role,
    DateTime CreatedAt);