namespace TicketCanvas.Common.Application.IntegrationEvents;

public record PaymentCompleted
(
    Guid OrderId
);