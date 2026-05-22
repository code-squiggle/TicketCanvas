using MediatR;
using TicketCanvas.Common.Application;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Application.Queries;

public record GetOrdersQuery
(
    Guid? UserId,
    Guid CurrentUserId,
    UserRole UserRole
) : IRequest<IEnumerable<OrderSummaryResponse>>;