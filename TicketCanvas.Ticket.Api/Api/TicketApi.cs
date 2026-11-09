using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TicketCanvas.Common.Http.Extensions;
using TicketCanvas.Ticket.Application.Dtos;
using TicketCanvas.Ticket.Application.Queries;

namespace TicketCanvas.Ticket.Api.Api;

public static class TicketApi
{
    public static IEndpointRouteBuilder MapTicketApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("tickets");
        api.MapGet("/{id:guid}", GetTicket).RequireAuthorization().WithName("GetTicket");
        api.MapGet("/", GetTickets).RequireAuthorization().WithName("GetTickets");
        return app;
    }

    public static async Task<Results<Ok<TicketDetailResponse>, NotFound>> GetTicket(
        Guid id,
        ClaimsPrincipal claimsPrincipal,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetTicketQuery(id, claimsPrincipal.GetUserId(), claimsPrincipal.GetUserRole());
        var order = await mediator.Send(query, cancellationToken);

        if (order == null)
            return TypedResults.NotFound();
 
        return TypedResults.Ok(order);
    }

    public static async Task<Ok<IEnumerable<TicketSummaryResponse>>> GetTickets(
        Guid? userId,
        Guid? orderId,
        ClaimsPrincipal claimsPrincipal,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetTicketsQuery(userId, orderId, claimsPrincipal.GetUserId(), claimsPrincipal.GetUserRole());

        var tickets = await mediator.Send(query, cancellationToken);

        return TypedResults.Ok(tickets);
    }
}
