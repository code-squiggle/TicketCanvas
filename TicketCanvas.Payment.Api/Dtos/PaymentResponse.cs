using TicketCanvas.Payment.Data.Models;

namespace TicketCanvas.Payment.Api.Dtos;

public record PaymentResponse
(
    Guid Id,
    Guid OrderId,
    PaymentStatus Status,
    string? TransactionId,
    string? FailureReason,
    DateTime ProcessedAt
);