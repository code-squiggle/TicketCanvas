namespace TicketCanvas.Ticket.Domain.Aggregates;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    
}

