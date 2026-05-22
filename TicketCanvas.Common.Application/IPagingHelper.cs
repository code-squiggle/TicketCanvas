using System.Linq.Expressions;
using TicketCanvas.Common.Application.Dtos;

namespace TicketCanvas.Common.Application;

public interface IPagingHelper
{
    Task<PagedResponse<TDto>> GetPagedItems<TEntity, TDto>(
        PagedRequest request,
        IQueryable<TEntity> queryable, 
        IReadOnlyDictionary<string, Expression<Func<TEntity, object?>>> sortDictionary,
        string defaultSortBy,
        string defaultSortDir,
        CancellationToken cancellationToken);
}
