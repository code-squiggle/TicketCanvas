using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Show.Api.Dto;
using TicketCanvas.Show.Data;
using TicketCanvas.Show.Data.Models;

namespace TicketCanvas.Show.Api.Api;

public static class TicketTypeApi
{
    public static IEndpointRouteBuilder MapTicketTypeApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("shows/{showId:guid}/ticketTypes");
        api.MapGet("/", GetTicketTypes).WithName("GetTicketTypes");
        api.MapPost("/", CreateTicketType).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("CreateTicketType");
        api.MapPut("/{id:guid}", UpdateTicketType).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("UpdateTicketType");
        api.MapDelete("/{id:guid}", DeleteTicketType).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("DeleteTicketType");

        return app;
    }

    public static async Task<Ok<List<TicketTypeResponse>>> GetTicketTypes(
        Guid showId, ShowDbContext showDbContext, IMapper mapper)
    {
        var ticketTypes = await showDbContext.TicketTypes
            .Where(ticketType => ticketType.ShowId == showId).ToListAsync();

        var ticketTypeDtos = mapper.Map<List<TicketTypeResponse>>(ticketTypes);
        
        return TypedResults.Ok(ticketTypeDtos);
    }

    public static async Task<Results<CreatedAtRoute<TicketTypeResponse>, NotFound>> CreateTicketType(
        Guid showId, CreateTicketTypeRequest createTicketTypeRequest, ShowDbContext showDbContext, IMapper mapper)
    {
        if (!await showDbContext.Shows.AnyAsync(show => show.Id == showId))
            return TypedResults.NotFound();

        var ticketType = mapper.Map<TicketType>(createTicketTypeRequest);
        ticketType.ShowId = showId;
        showDbContext.TicketTypes.Add(ticketType);
        await showDbContext.SaveChangesAsync();

        var ticketTypeDto = mapper.Map<TicketTypeResponse>(ticketType);

        return TypedResults.CreatedAtRoute(ticketTypeDto, "GetShow", new { Id = showId });
    }

    public static async Task<Results<Ok<TicketTypeResponse>, NotFound>> UpdateTicketType(
        Guid id, UpdateTicketTypeRequest updateTicketTypeRequest, ShowDbContext showDbContext, IMapper mapper)
    {
        var ticketType = await showDbContext.TicketTypes.SingleOrDefaultAsync(ticketType => ticketType.Id == id);

        if (ticketType == null)
            return TypedResults.NotFound();

        mapper.Map(updateTicketTypeRequest, ticketType);
        await showDbContext.SaveChangesAsync();

        var ticketTypeDto = mapper.Map<TicketTypeResponse>(ticketType);

        return TypedResults.Ok(ticketTypeDto);
    }

    public static async Task<Results<NoContent, NotFound>> DeleteTicketType(
        Guid id, ShowDbContext showDbContext, IMapper mapper)
    {
        var deletedCount = await showDbContext.TicketTypes.Where(ticketType => ticketType.Id == id).ExecuteDeleteAsync();
        if (deletedCount == 0)
            return TypedResults.NotFound();

        return TypedResults.NoContent();
    }
}
