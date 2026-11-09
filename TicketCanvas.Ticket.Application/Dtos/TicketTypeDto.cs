namespace TicketCanvas.Ticket.Application.Dtos;

public record TicketTypeDto
(
    Guid Id,
    string Name,
    decimal Price,
    string Currency,
    int TotalQuantity
);