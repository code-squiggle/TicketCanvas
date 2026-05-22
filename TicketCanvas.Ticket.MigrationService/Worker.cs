using Microsoft.EntityFrameworkCore;
using TicketCanvas.Ticket.Domain.Aggregates;
using TicketCanvas.Ticket.Domain.ValueObjects;
using TicketCanvas.Ticket.Infrastructure.Persistence.Write;

namespace TicketCanvas.Ticket.MigrationService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{ 
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TicketDbContext>();

        await RunMigration(dbContext, cancellationToken);
        await SeedData(dbContext, cancellationToken);

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigration(TicketDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedData(TicketDbContext dbContext, CancellationToken cancellationToken)
    {
        var ticketAllocations = new List<TicketAllocation>
        {
            TicketAllocation.Create(
                id: Guid.Parse("54ba835f-a35d-43fb-8f3f-307928334b4e"),
                showId: Guid.Parse("03ea65e3-2c3e-45dc-8495-8eb42b7e53de"),
                showName: "Show",
                ticketTypeName: "TicketType",
                price: 8,
                currency: Currency.USD.ToString(),
                quantity: 8686),

            TicketAllocation.Create(
                id: Guid.Parse("ec48fa1e-ced7-4535-bb91-73964a3e9d9a"),
                showId: Guid.Parse("03ea65e3-2c3e-45dc-8495-8eb42b7e53de"),
                showName: "Show",
                ticketTypeName: "Student",
                price: 1,
                currency: Currency.USD.ToString(),
                quantity: 8686),
        };

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            if (await dbContext.TicketAllocations.AnyAsync())
            {
                await transaction.RollbackAsync(cancellationToken);
                return;
            }
            await dbContext.TicketAllocations.AddRangeAsync(ticketAllocations, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}