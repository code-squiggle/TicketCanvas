using TicketCanvas.Ticket.MigrationService;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Ticket.Infrastructure.Persistence.Write;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("TicketMigration"));
builder.AddSqlServerDbContext<TicketDbContext>("ticket-db", configureDbContextOptions: options =>
{
    options.ConfigureSqlEngine(o => o.MigrationsAssembly("TicketCanvas.Ticket.MigrationService"));
});

var host = builder.Build();
host.Run();
