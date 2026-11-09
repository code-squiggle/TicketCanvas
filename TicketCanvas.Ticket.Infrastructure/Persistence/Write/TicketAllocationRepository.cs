using Microsoft.EntityFrameworkCore;
using TicketCanvas.Ticket.Application.Repositories;
using TicketCanvas.Ticket.Domain.Aggregates;

namespace TicketCanvas.Ticket.Infrastructure.Persistence.Write;

public class TicketAllocationRepository : ITicketAllocationRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public TicketAllocationRepository(TicketDbContext ticketDbContext)
    {
        _ticketDbContext = ticketDbContext;
    }

    public void AddRange(IEnumerable<TicketAllocation> ticketAllocations)
    {
        _ticketDbContext.TicketAllocations.AddRange(ticketAllocations);
    }

    public async Task<List<TicketAllocation>> GetByIds(IReadOnlyCollection<Guid> ids)
    {
        return await _ticketDbContext.TicketAllocations.Where(ticketAllocation => ids.Contains(ticketAllocation.Id)).ToListAsync();
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _ticketDbContext.SaveChangesAsync(cancellationToken);
    }
}
