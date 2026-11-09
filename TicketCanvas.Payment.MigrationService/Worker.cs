using Microsoft.EntityFrameworkCore;
using TicketCanvas.Payment.Data;

namespace TicketCanvas.Payment.MigrationService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{ 
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

        await RunMigration(dbContext, cancellationToken);
        await SeedData(dbContext, cancellationToken);

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigration(PaymentDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedData(PaymentDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            // await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}