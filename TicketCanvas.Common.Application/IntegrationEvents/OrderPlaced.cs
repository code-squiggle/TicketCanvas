namespace TicketCanvas.Common.Application.IntegrationEvents;

public record OrderPlaced
(
    Guid Id,
    Guid UserId,
    decimal TotalAmount,
    string Currency,
    string CardToken
);