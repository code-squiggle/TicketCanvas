using Microsoft.EntityFrameworkCore;
using TicketCanvas.Payment.Data;
using TicketCanvas.Payment.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("PaymentMigration"));
builder.AddSqlServerDbContext<PaymentDbContext>("payment-db", configureDbContextOptions: options =>
{
    options.ConfigureSqlEngine(o => o.MigrationsAssembly("TicketCanvas.Payment.MigrationService"));
});

var host = builder.Build();
host.Run();
