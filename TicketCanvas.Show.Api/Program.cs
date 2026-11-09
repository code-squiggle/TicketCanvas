using System.Text.Json.Serialization;
using MassTransit;
using TicketCanvas.Common.Application;
using TicketCanvas.Common.Infrastructure;
using TicketCanvas.Show.Api.Api;
using TicketCanvas.Show.Api.MappingProfiles;
using TicketCanvas.Show.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IPagingHelper, PagingHelper>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.AddSqlServerDbContext<ShowDbContext>("show-db");

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddEntityFrameworkOutbox<ShowDbContext>(cfg =>
    {
        cfg.UseSqlServer();
        cfg.UseBusOutbox();
    });
    cfg.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalar();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

app.MapShowApi();
app.MapTicketTypeApi();
app.MapVenueApi();

app.Run();