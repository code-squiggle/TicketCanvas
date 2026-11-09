using TicketCanvas.Ticket.Domain.Aggregates;
using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Application.Dtos;

public record OrderDetailResponse(
    Guid Id,
    OrderStatus Status,
    decimal TotalAmount,
    Currency Currency,
    List<OrderItemResponse> OrderItems);