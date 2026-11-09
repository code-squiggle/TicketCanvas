namespace TicketCanvas.Common.Application.IntegrationEvents;

public record TicketType
(
    Guid Id,
    string Name,
    decimal Price,
    string Currency,
    int TotalQuantity
);
