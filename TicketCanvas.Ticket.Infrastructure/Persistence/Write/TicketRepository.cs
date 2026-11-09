using TicketCanvas.Ticket.Application.Repositories;
using TicketModel = TicketCanvas.Ticket.Domain.Aggregates.Ticket;

namespace TicketCanvas.Ticket.Infrastructure.Persistence.Write;

public class TicketRepository : ITicketRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public TicketRepository(TicketDbContext ticketDbContext)
    {
        _ticketDbContext = ticketDbContext;
    }

    public void AddRange(IEnumerable<TicketModel> tickets)
    {
        _ticketDbContext.Tickets.AddRange(tickets);
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _ticketDbContext.SaveChangesAsync(cancellationToken);
    }
}
