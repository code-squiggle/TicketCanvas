namespace TicketCanvas.Payment.Data.Models;

public class Payment : Entity
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime ProcessedAt { get; set; }
}
