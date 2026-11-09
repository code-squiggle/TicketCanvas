using Microsoft.EntityFrameworkCore;
using TicketCanvas.Show.Data;
using TicketCanvas.Show.Data.Models;
using ShowModel = TicketCanvas.Show.Data.Models.Show;

namespace TicketCanvas.Show.MigrationService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{ 
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShowDbContext>();

        await RunMigration(dbContext, cancellationToken);
        await SeedData(dbContext, cancellationToken);

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigration(ShowDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedData(ShowDbContext dbContext, CancellationToken cancellationToken)
    {
        var venue = new Venue()
        {
            Id = Guid.Parse("578366a1-830f-4b8b-afec-935865ad747c"),
            Name = "Venue",
            Address = "Address",
            City = "City",
            Capacity = 868686,
        };

        var show = new ShowModel()
        {
            Id = Guid.Parse("03ea65e3-2c3e-45dc-8495-8eb42b7e53de"),
            Name = "Show",
            VenueId = venue.Id,
            Venue = venue
        };

        var ticketTypes = new List<TicketType> {
            new() {
                Id = Guid.Parse("54ba835f-a35d-43fb-8f3f-307928334b4e"),
                Name = "TicketType",
                Price = 8,
                TotalQuantity = 8686,
                Show = show
            },
            new() {
                Id = Guid.Parse("ec48fa1e-ced7-4535-bb91-73964a3e9d9a"),
                Name = "Student",
                Price = 1,
                TotalQuantity = 8686,
                Show = show
            }
        };

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            if (await dbContext.Shows.AnyAsync())
            {
                await transaction.RollbackAsync(cancellationToken);
                return;
            }
            await dbContext.Venues.AddAsync(venue, cancellationToken);
            await dbContext.Shows.AddAsync(show, cancellationToken);
            await dbContext.TicketTypes.AddRangeAsync(ticketTypes, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}