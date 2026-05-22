using TicketCanvas.Common.Application;
using TicketCanvas.User.Data.Models;

namespace TicketCanvas.User.Api.Dtos;

public record UpdateUserRoleRequest(UserRole Role);