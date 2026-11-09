using TicketCanvas.Ticket.Domain.Aggregates;

namespace TicketCanvas.Ticket.Application.Repositories;

public interface IOrderRepository
{
    void Add(Order order);
    Task<Order?> GetById(Guid id);
    Task SaveChanges(CancellationToken cancellationToken = default);
}
