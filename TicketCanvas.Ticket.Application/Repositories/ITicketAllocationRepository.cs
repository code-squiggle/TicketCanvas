using TicketCanvas.Ticket.Domain.Aggregates;

namespace TicketCanvas.Ticket.Application.Repositories;

public interface ITicketAllocationRepository
{
    Task<List<TicketAllocation>> GetByIds(IReadOnlyCollection<Guid> ids);
    void AddRange(IEnumerable<TicketAllocation> ticketAllocations);
    Task SaveChanges(CancellationToken cancellationToken = default);
}
