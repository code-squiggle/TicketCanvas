namespace TicketCanvas.Common.Application.Dtos;

public record PagedResponse<T>
(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int PagesCount
);