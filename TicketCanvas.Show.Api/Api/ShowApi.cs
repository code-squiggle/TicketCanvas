using System.Linq.Expressions;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Common.Application;
using TicketCanvas.Common.Application.Dtos;
using TicketCanvas.Common.Application.IntegrationEvents;
using TicketCanvas.Show.Api.Dto;
using TicketCanvas.Show.Api.Dtos;
using TicketCanvas.Show.Data;
using TicketCanvas.Show.Data.Models;
using ShowModel = TicketCanvas.Show.Data.Models.Show;

namespace TicketCanvas.Show.Api.Api;

public static class ShowApi
{
    public static IEndpointRouteBuilder MapShowApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("shows");
        api.MapGet("/", GetShows).WithName("GetShows");
        api.MapGet("/{id:guid}", GetShow).WithName("GetShow");
        api.MapPost("/", CreateShow).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("CreateShow");
        api.MapPut("/{id:guid}", UpdateShow).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("UpdateShow");
        api.MapPatch("/{id:guid}/status", UpdateShowStatus).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("UpdateShowStatus");
        api.MapDelete("/{id:guid}", DeleteShow).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("DeleteShow");

        return app;
    }

    public static async Task<Results<Ok<ShowDetailResponse>, NotFound>> GetShow(
        Guid id, ShowDbContext showDbContext, IMapper mapper)
    {
        var show = await showDbContext.Shows
            .Include(show => show.TicketTypes)
            .Include(show => show.Venue)
            .SingleOrDefaultAsync(show => show.Id == id);

        if (show == null)
            return TypedResults.NotFound();

        var showDetail = mapper.Map<ShowDetailResponse>(show);
        
        return TypedResults.Ok(showDetail);
    }

    public static async Task<Ok<PagedResponse<ShowSummaryResponse>>> GetShows(
        [AsParameters] ShowsRequest request,
        ShowDbContext showDbContext, 
        [FromServices] IPagingHelper pagingHelper,
        CancellationToken cancellationToken)
    {
        var queryableShows = showDbContext.Shows.Include(show => show.Venue).AsQueryable();

        if (request.Status != null)
            queryableShows = queryableShows.Where(show => show.Venue.City == request.City);

        if (request.Status != null)
            queryableShows = queryableShows.Where(show => show.Status == request.Status);

        if (request.FromDate != null)
            queryableShows = queryableShows.Where(show => show.StartDateTime >= request.FromDate);

        if (request.ToDate != null)
            queryableShows = queryableShows.Where(show => show.StartDateTime <= request.ToDate);

        var sortDictionary = new Dictionary<string, Expression<Func<ShowModel, object?>>>
        {
            ["name"] = show => show.Name,
            ["venue"] = show => show.Venue.Name,
            ["startDateTime"] = show => show.StartDateTime,
            ["status"] = show => show.Status,
            ["createdAt"] = show => show.CreatedAt,
        };

        var result = await pagingHelper.GetPagedItems<ShowModel, ShowSummaryResponse>(
            request,
            queryableShows,
            sortDictionary,
            "startDateTime",
            "desc",
            cancellationToken);

        return TypedResults.Ok(result);
    }

    public static async Task<CreatedAtRoute<ShowDetailResponse>> CreateShow(
        CreateShowRequest createShowRequest, ShowDbContext showDbContext, IMapper mapper)
    {
        var show = mapper.Map<ShowModel>(createShowRequest);
        show.Status = ShowStatus.Draft;
        showDbContext.Shows.Add(show);
        await showDbContext.SaveChangesAsync();

        var showDetail = mapper.Map<ShowDetailResponse>(show);

        return TypedResults.CreatedAtRoute(showDetail, "GetShow", new { id = show.Id });
    }

    public static async Task<Results<Ok<ShowDetailResponse>, NotFound>> UpdateShow(
        Guid id, UpdateShowRequest updateShowRequest, ShowDbContext showDbContext, IMapper mapper)
    {
        var show = await showDbContext.Shows
            .Include(show => show.Venue)
            .Include(show => show.TicketTypes)
            .SingleOrDefaultAsync(show => show.Id == id);

        if (show == null)
            return TypedResults.NotFound();

        mapper.Map(updateShowRequest, show);
        await showDbContext.SaveChangesAsync();

        var showDetail = mapper.Map<ShowDetailResponse>(show);

        return TypedResults.Ok(showDetail);
    }

    public static async Task<Results<Ok, NotFound>> UpdateShowStatus(
        Guid id,
        UpdateShowStatusRequest updateShowStatusRequest,
        ShowDbContext showDbContext,
        IMapper mapper,
        IPublishEndpoint publishEndpoint)
    {
        var show = await showDbContext.Shows
            .Include(show => show.TicketTypes)
            .SingleOrDefaultAsync(show => show.Id == id);

        if (show == null)
            return TypedResults.NotFound();

        show.Status = updateShowStatusRequest.Status;
        if (show.Status == ShowStatus.Published)
        {
            var showPublished = mapper.Map<ShowPublished>(show);
            await publishEndpoint.Publish(showPublished);
        }
        await showDbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }

    public static async Task<Results<Ok, NotFound>> DeleteShow(
        Guid id, ShowDbContext showDbContext, IMapper mapper)
    {
        var deletedCount = await showDbContext.Shows.Where(show => show.Id == id).ExecuteDeleteAsync();

        if (deletedCount == 0)
            return TypedResults.NotFound();

        return TypedResults.Ok();
    }
}
