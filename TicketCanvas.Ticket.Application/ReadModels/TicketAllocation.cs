using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Application.ReadModels;

public class TicketAllocation
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid ShowId { get; private set; }
    public string ShowName { get; private set; } = string.Empty;
    public string TicketTypeName { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public Currency Currency { get; private set; }
    public int TotalQuantity { get; private set; }
    public int AvailableQuantity { get; private set; }
}
