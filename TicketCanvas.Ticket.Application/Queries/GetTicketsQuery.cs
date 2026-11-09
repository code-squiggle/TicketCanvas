using MediatR;
using TicketCanvas.Common.Application;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Application.Queries;

public record GetTicketsQuery
(
    Guid? UserId,
    Guid? OrderId,
    Guid CurrentUserId,
    UserRole UserRole
) : IRequest<IEnumerable<TicketSummaryResponse>>;