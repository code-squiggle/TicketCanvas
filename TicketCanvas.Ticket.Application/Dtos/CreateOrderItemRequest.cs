using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Application.Dtos;

public record CreateOrderItemRequest
(
    Guid TicketTypeId,
    int Quantity,
    decimal ExpectedPrice,
    Currency Currency
);