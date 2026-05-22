using Microsoft.EntityFrameworkCore;
using TicketCanvas.Ticket.Application.Repositories;
using TicketCanvas.Ticket.Domain.Aggregates;

namespace TicketCanvas.Ticket.Infrastructure.Persistence.Write;

public class OrderRepository : IOrderRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public OrderRepository(TicketDbContext ticketDbContext)
    {
        _ticketDbContext = ticketDbContext;
    }

    public void Add(Order order)
    {
        _ticketDbContext.Orders.Add(order);
    }

    public async Task<Order?> GetById(Guid id)
    {
        return await _ticketDbContext.Orders
            .Include(order => order.OrderItems)
            .SingleOrDefaultAsync(order => order.Id == id);
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _ticketDbContext.SaveChangesAsync(cancellationToken);
    }
}
