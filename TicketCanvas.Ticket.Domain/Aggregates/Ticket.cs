using TicketCanvas.Common.Domain.Exceptions;

namespace TicketCanvas.Ticket.Domain.Aggregates;

public class Ticket : AggregateRoot
{
    public Guid OrderItemId { get; private set; }
    public string QRCode { get; private set; } = string.Empty;
    public DateTime IssuedAt { get; private set; }

    public static Ticket Create(
        Guid orderItemId,
        string qrCode,
        DateTime issuedAt)
    {
        if (string.IsNullOrEmpty(qrCode))
            throw new DomainException("QR Code must not be empty.");

        if (issuedAt > DateTime.UtcNow)
            throw new DomainException("IssuedAt must be in the past.");

        var ticket = new Ticket
        {
            Id = Guid.CreateVersion7(),
            OrderItemId = orderItemId,
            QRCode = qrCode,
            IssuedAt = issuedAt,
        };

        return ticket;
    }
}
