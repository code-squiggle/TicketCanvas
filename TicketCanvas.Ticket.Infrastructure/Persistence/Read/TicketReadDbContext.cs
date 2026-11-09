using Microsoft.EntityFrameworkCore;
using TicketCanvas.Ticket.Application.DbContexts;
using TicketCanvas.Ticket.Application.ReadModels;
using TicketModel = TicketCanvas.Ticket.Application.ReadModels.Ticket;

namespace TicketCanvas.Ticket.Infrastructure.Persistence.Read;

public class TicketReadDbContext(DbContextOptions<TicketReadDbContext> options) : DbContext(options), ITicketReadContext
{
    public IQueryable<TicketModel> Tickets => Set<TicketModel>().AsNoTracking();
    public IQueryable<TicketAllocation> TicketAllocations => Set<TicketAllocation>().AsNoTracking();
    public IQueryable<Order> Orders => Set<Order>().AsNoTracking();
    public IQueryable<OrderItem> OrderItems => Set<OrderItem>().AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TicketModel>().ToTable("Tickets");
        modelBuilder.Entity<TicketAllocation>().ToTable("TicketAllocations");
        modelBuilder.Entity<Order>().ToTable("Orders");
        modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        
        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>();
    }
}
