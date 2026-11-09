using System.Text.Json.Serialization;
using MassTransit;
using TicketCanvas.Payment.Api.Api;
using TicketCanvas.Payment.Api.IntegrationEventConsumers;
using TicketCanvas.Payment.Api.MappingProfiles;
using TicketCanvas.Payment.Api.PaymentProcessor;
using TicketCanvas.Payment.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddScoped<IPaymentProcessor, MockPaymentProcessor>();

builder.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.AddSqlServerDbContext<PaymentDbContext>("payment-db");

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddEntityFrameworkOutbox<PaymentDbContext>(cfg =>
    {
        cfg.UseSqlServer();
        cfg.UseBusOutbox();
    });

    cfg.AddConsumer<OrderPlacedConsumer>();

    cfg.AddConfigureEndpointsCallback((context, name, cfg) =>
    {
        cfg.UseEntityFrameworkOutbox<PaymentDbContext>(context);
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

app.MapPaymentApi();

app.Run();