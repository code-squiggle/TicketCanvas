using TicketCanvas.Ticket.Application.ReadModels;
using TicketModel = TicketCanvas.Ticket.Application.ReadModels.Ticket;

namespace TicketCanvas.Ticket.Application.DbContexts;

public interface ITicketReadContext
{
    IQueryable<Order> Orders { get; }
    IQueryable<TicketModel> Tickets { get; }
}
