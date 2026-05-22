namespace TicketCanvas.Common.Application.IntegrationEvents;

public record PaymentFailed
(
    Guid OrderId,
    string? FailureReason
);