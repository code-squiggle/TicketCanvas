using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Application.ReadModels;

public class Order
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid UserId { get; private set; }
    public Guid IdempotencyKey { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public Currency Currency { get; private set; }
    public List<OrderItem> OrderItems { get; private set; } = [];
}

