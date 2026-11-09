using TicketCanvas.Common.Application;
using TicketCanvas.Common.Application.Dtos;

namespace TicketCanvas.User.Api.Dtos;

public record GetUsersRequest : PagedRequest
{
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public UserRole? Role { get; init; }
 }