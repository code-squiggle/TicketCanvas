using TicketModel = TicketCanvas.Ticket.Domain.Aggregates.Ticket;

namespace TicketCanvas.Ticket.Application.Repositories;

public interface ITicketRepository
{
    void AddRange(IEnumerable<TicketModel> tickets);
    Task SaveChanges(CancellationToken cancellationToken = default);
}
