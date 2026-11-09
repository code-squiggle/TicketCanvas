using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Application.ReadModels;

public class OrderItem
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public Guid OrderId { get; private set; }
    public Guid ShowId { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public string ShowName { get; private set; } = string.Empty;
    public string TicketTypeName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public Currency Currency { get; private set; }
    public Order Order { get; private set; } = new();
}
