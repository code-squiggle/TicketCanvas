using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TicketCanvas.Common.Http.Extensions;
using TicketCanvas.Ticket.Application.Commands;
using TicketCanvas.Ticket.Application.Dtos;
using TicketCanvas.Ticket.Application.Queries;

namespace TicketCanvas.Ticket.Api.Api;

public static class OrderApi
{
    public static IEndpointRouteBuilder MapOrderApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("orders");
        api.MapGet("/", GetOrders).RequireAuthorization().WithName("GetOrders");
        api.MapGet("/{id:guid}", GetOrder).RequireAuthorization().WithName("GetOrder");
        api.MapPost("/", CreateOrder).RequireAuthorization().WithName("CreateOrder");
        return app;
    }

    public static async Task<Ok<IEnumerable<OrderSummaryResponse>>> GetOrders(
        Guid? userId,
        ClaimsPrincipal claimsPrincipal,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetOrdersQuery(userId, claimsPrincipal.GetUserId(), claimsPrincipal.GetUserRole());
        var orders = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(orders);
    }

    public static async Task<IResult> GetOrder(
        Guid id,
        ClaimsPrincipal claimsPrincipal,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderQuery(id, claimsPrincipal.GetUserId(), claimsPrincipal.GetUserRole());
        var orderResult = await mediator.Send(query, cancellationToken);
        return orderResult.GetHttpResult();
    }

    public static async Task<IResult> CreateOrder(
        Guid idempotencyKey,
        CreateOrderRequest createOrderRequest,
        ClaimsPrincipal claimsPrincipal,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(
            claimsPrincipal.GetUserId(),
            idempotencyKey,
            createOrderRequest.OrderItems,
            createOrderRequest.CardToken);
        var orderIdResult = await mediator.Send(command, cancellationToken);
        return orderIdResult.GetHttpResult("GetOrder", new { Id = orderIdResult.Value });
    }
}