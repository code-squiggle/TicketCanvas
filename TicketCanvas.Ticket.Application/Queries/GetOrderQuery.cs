using MediatR;
using TicketCanvas.Common;
using TicketCanvas.Common.Application;
using TicketCanvas.Common.Domain.Results;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Application.Queries;

public record GetOrderQuery
(
    Guid OrderId,
    Guid UserId,
    UserRole UserRole
) : IRequest<Result<OrderDetailResponse>>;