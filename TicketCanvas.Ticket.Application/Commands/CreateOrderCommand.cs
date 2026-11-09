using MediatR;
using TicketCanvas.Common.Domain.Results;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Application.Commands;

public record CreateOrderCommand
(
    Guid UserId,
    Guid IdempotencyKey,
    IReadOnlyList<CreateOrderItemRequest> OrderItems,
    string CardToken
) : IRequest<Result<Guid>>;