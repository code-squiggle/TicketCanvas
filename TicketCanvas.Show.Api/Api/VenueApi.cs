using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Common.Application;
using TicketCanvas.Common.Application.Dtos;
using TicketCanvas.Show.Api.Dto;
using TicketCanvas.Show.Api.Dtos;
using TicketCanvas.Show.Data;
using TicketCanvas.Show.Data.Models;

namespace TicketCanvas.Show.Api.Api;

public static class VenueApi
{
    public static IEndpointRouteBuilder MapVenueApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("venues");
        api.MapGet("/", GetVenues).WithName("GetVenues");
        api.MapGet("/{id:guid}", GetVenue).WithName("GetVenue");
        api.MapPost("/", CreateVenue).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("CreateVenue");
        api.MapPut("/{id:guid}", UpdateVenue).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("UpdateVenue");
        api.MapDelete("/{id:guid}", DeleteVenue).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("DeleteVenue");

        return app;
    }

    public static async Task<Results<Ok<VenueResponse>, NotFound>> GetVenue(
        Guid id, ShowDbContext showDbContext, IMapper mapper)
    {
        var venue = await showDbContext.Venues.SingleOrDefaultAsync(venue => venue.Id == id);

        if (venue == null)
            return TypedResults.NotFound();

        var venueDto = mapper.Map<VenueResponse>(venue);
        
        return TypedResults.Ok(venueDto);
    }

    public static async Task<Ok<PagedResponse<VenueResponse>>> GetVenues(
        [AsParameters] VenuesRequest request,
        ShowDbContext showDbContext,
        [FromServices] IPagingHelper pagingHelper,
        CancellationToken cancellationToken)
    {
        var queryableVenues = showDbContext.Venues.AsQueryable();

        var sortDictionary = new Dictionary<string, Expression<Func<Venue, object?>>>
        {
            ["name"] = venue => venue.Name,
            ["address"] = venue => venue.Address,
            ["city"] = venue => venue.City,
            ["capacity"] = venue => venue.Capacity,
            ["createdAt"] = venue => venue.CreatedAt,
        };

        var result = await pagingHelper.GetPagedItems<Venue, VenueResponse>(
            request,
            queryableVenues,
            sortDictionary,
            "createdAt",
            "desc",
            cancellationToken);

        return TypedResults.Ok(result);
    }

    public static async Task<CreatedAtRoute<VenueResponse>> CreateVenue(
        CreateVenueRequest createVenueRequest, ShowDbContext showDbContext, IMapper mapper)
    {
        var venue = mapper.Map<Venue>(createVenueRequest);
        showDbContext.Venues.Add(venue);
        await showDbContext.SaveChangesAsync();

        var venueDto = mapper.Map<VenueResponse>(venue);

        return TypedResults.CreatedAtRoute(venueDto, "GetShow", venue.Id);
    }

    public static async Task<Results<Ok<VenueResponse>, NotFound>> UpdateVenue(
        Guid id, UpdateVenueRequest updateVenueRequest, ShowDbContext showDbContext, IMapper mapper)
    {
        var venue = await showDbContext.Venues.SingleOrDefaultAsync(venue => venue.Id == id);

        if (venue == null)
            return TypedResults.NotFound();

        mapper.Map(updateVenueRequest, venue);
        await showDbContext.SaveChangesAsync();

        var venueDto = mapper.Map<VenueResponse>(venue);

        return TypedResults.Ok(venueDto);
    }

    public static async Task<Results<Ok, NotFound>> DeleteVenue(
        Guid id, ShowDbContext showDbContext, IMapper mapper)
    {
        var deletedCount = await showDbContext.Venues.Where(venue => venue.Id == id).ExecuteDeleteAsync();

        if (deletedCount == 0)
            return TypedResults.NotFound();

        return TypedResults.Ok();
    }
}
