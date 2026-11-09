using TicketCanvas.Show.MigrationService;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Show.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("ShowMigration"));
builder.AddSqlServerDbContext<ShowDbContext>("show-db", configureDbContextOptions: options =>
{
    options.ConfigureSqlEngine(o => o.MigrationsAssembly("TicketCanvas.Show.MigrationService"));
});

var host = builder.Build();
host.Run();
