using TicketCanvas.Ticket.Domain.Aggregates;
using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Domain.Dtos;

public record OrderItemDto
(
    TicketAllocation TicketAllocation,
    int Quantity,
    Money ExpectedPrice
);