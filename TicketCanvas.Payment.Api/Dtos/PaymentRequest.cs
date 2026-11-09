namespace TicketCanvas.Payment.Api.Dtos;

public record PaymentRequest(
    Guid OrderId,
    decimal Amount,
    string CardToken,
    string IdempotencyKey
);