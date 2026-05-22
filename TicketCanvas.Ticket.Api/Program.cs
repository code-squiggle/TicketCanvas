using System.Text.Json.Serialization;
using FluentValidation;
using MassTransit;
using TicketCanvas.Common.Application;
using TicketCanvas.Common.Http.Extensions;
using TicketCanvas.Common.Infrastructure;
using TicketCanvas.Ticket.Api.Api;
using TicketCanvas.Ticket.Application.DbContexts;
using TicketCanvas.Ticket.Application.MappingProfiles;
using TicketCanvas.Ticket.Application.Repositories;
using TicketCanvas.Ticket.Application.Validation;
using TicketCanvas.Ticket.Infrastructure.IntegrationEventConsumers;
using TicketCanvas.Ticket.Infrastructure.Persistence.Read;
using TicketCanvas.Ticket.Infrastructure.Persistence.Write;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddScoped<IPagingHelper, PagingHelper>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddValidatorsFromAssembly(TicketCanvas.Ticket.Application.AssemblyReference.Assembly);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(TicketCanvas.Ticket.Application.AssemblyReference.Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ITicketAllocationRepository, TicketAllocationRepository>();

builder.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.AddSqlServerDbContext<TicketDbContext>("ticket-db");
builder.AddSqlServerDbContext<TicketReadDbContext>("ticket-db");
builder.Services.AddScoped<ITicketReadContext>(sp => sp.GetRequiredService<TicketReadDbContext>());

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddEntityFrameworkOutbox<TicketDbContext>(cfg =>
    {
        cfg.UseSqlServer();
        cfg.UseBusOutbox();
    });

    cfg.AddConsumer<ShowPublishedConsumer>();
    cfg.AddConsumer<PaymentCompletedConsumer>();
    cfg.AddConsumer<PaymentFailedConsumer>();

    cfg.AddConfigureEndpointsCallback((context, name, cfg) =>
    {
        cfg.UseEntityFrameworkOutbox<TicketDbContext>(context);
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

app.MapOrderApi();
app.MapTicketApi();

app.Run();