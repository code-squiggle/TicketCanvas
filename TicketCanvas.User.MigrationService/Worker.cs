using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Common.Application;
using TicketCanvas.User.Data;
using UserModel = TicketCanvas.User.Data.Models.User;

namespace TicketCanvas.User.MigrationService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{ 
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

        await RunMigration(dbContext, cancellationToken);
        await SeedData(dbContext, cancellationToken);

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigration(UserDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedData(UserDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            if (await dbContext.Users.AnyAsync())
            {
                await transaction.RollbackAsync(cancellationToken);
                return;
            }

            var passwordHasher = new PasswordHasher<UserModel>();
            var admin = new UserModel
            {
                Id = Guid.Parse("20596bc1-7609-4a5e-9fc2-cfe0ba399489"),
                Email = "ticketcanvas.admin@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = UserRole.Admin,
            };
            admin.PasswordHash = passwordHasher.HashPassword(admin, "1234");
            dbContext.Users.Add(admin);

            var customer = new UserModel
            {
                Id = Guid.Parse("3dcc6814-e3a8-46be-a209-1ff6a49adb99"),
                Email = "ticketcanvas.customer@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = UserRole.Customer,
            };
            customer.PasswordHash = passwordHasher.HashPassword(customer, "1234");
            dbContext.Users.Add(customer);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}