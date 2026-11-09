namespace TicketCanvas.Ticket.Application.ReadModels;

public class Ticket
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid OrderItemId { get; private set; }
    public string QRCode { get; private set; } = string.Empty;
    public DateTime IssuedAt { get; private set; }
    public OrderItem OrderItem { get; private set; } = new();
}
