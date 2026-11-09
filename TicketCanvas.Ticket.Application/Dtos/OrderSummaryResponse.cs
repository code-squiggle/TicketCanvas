using TicketCanvas.Ticket.Domain.Aggregates;
using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Application.Dtos;

public record OrderSummaryResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    OrderStatus Status,
    decimal TotalAmount,
    Currency Currency,
    DateTime CreatedAt,
    DateTime UpdatedAt);