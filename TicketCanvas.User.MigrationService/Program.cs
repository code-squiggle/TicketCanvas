using Microsoft.EntityFrameworkCore;
using TicketCanvas.User.Data;
using TicketCanvas.User.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("UserMigration"));
builder.AddSqlServerDbContext<UserDbContext>("user-db", configureDbContextOptions: options =>
{
    options.ConfigureSqlEngine(o => o.MigrationsAssembly("TicketCanvas.User.MigrationService"));
});

var host = builder.Build();
host.Run();
