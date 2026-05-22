namespace TicketCanvas.Common.Application.Dtos;

public record PagedRequest
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
    public string? SortBy { get; init; }
    public string? SortDir { get; init; }
};