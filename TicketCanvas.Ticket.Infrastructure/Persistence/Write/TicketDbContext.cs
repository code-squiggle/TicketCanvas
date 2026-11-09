using MassTransit;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Ticket.Domain.Aggregates;
using TicketCanvas.Ticket.Infrastructure.Persistence.Write.Configurations;
using TicketModel = TicketCanvas.Ticket.Domain.Aggregates.Ticket;

namespace TicketCanvas.Ticket.Infrastructure.Persistence.Write;

public class TicketDbContext(DbContextOptions<TicketDbContext> options) : DbContext(options)
{
    public DbSet<TicketModel> Tickets => Set<TicketModel>();
    public DbSet<TicketAllocation> TicketAllocations => Set<TicketAllocation>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new TicketAllocationConfiguration());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        
        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var entities = ChangeTracker.Entries<Entity>();

        foreach (var entry in entities)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(e => e.CreatedAt).CurrentValue = now;
                    break;
                case EntityState.Modified:
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    break;
            }
        }

        var aggregateRoots = ChangeTracker.Entries<AggregateRoot>();

        foreach (var entry in aggregateRoots)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(e => e.CreatedAt).CurrentValue = now;
                    entry.Property(e => e.UpdatedAt).CurrentValue = now;
                    break;
                case EntityState.Modified:
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.UpdatedAt).CurrentValue = now;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
