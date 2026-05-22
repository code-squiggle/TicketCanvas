using MediatR;
using TicketCanvas.Common;
using TicketCanvas.Common.Application;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Application.Queries;

public record GetTicketQuery
(
    Guid TicketId,
    Guid UserId,
    UserRole UserRole
) : IRequest<TicketDetailResponse?>;