namespace TicketCanvas.Payment.Api.Dtos;

public record PaymentResult(
    bool Succeeded,
    string TransactionId,
    string? FailureReason
);