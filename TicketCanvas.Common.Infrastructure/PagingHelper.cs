using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Common.Application;
using TicketCanvas.Common.Application.Dtos;

namespace TicketCanvas.Common.Infrastructure;

public class PagingHelper : IPagingHelper
{
    private readonly IMapper _mapper;

    private const int DefaultPage = 1;
    private const int DefaultPageSize = 5;

    public PagingHelper(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<PagedResponse<TDto>> GetPagedItems<TEntity, TDto>(
        PagedRequest request,
        IQueryable<TEntity> queryable,
        IReadOnlyDictionary<string, Expression<Func<TEntity, object?>>> sortDictionary,
        string defaultSortBy,
        string defaultSortDir,
        CancellationToken cancellationToken)
    {
        int page = request.Page ?? DefaultPage;
        int pageSize = request.PageSize ?? DefaultPageSize;

        int totalCount = await queryable.CountAsync(cancellationToken);

        int pagesCount = Math.Min(totalCount, (totalCount - 1) / pageSize + 1);
        page = Math.Min(page, pagesCount);

        string? sortBy = request.SortBy;
        string? sortDir = request.SortDir;

        if (string.IsNullOrEmpty(sortBy))
        {
            sortBy = defaultSortBy;
            sortDir = defaultSortDir;
        }

        var keySelector = sortDictionary.GetValueOrDefault(sortBy);

        if (keySelector == null)
            throw new ApplicationException("Invalid sort field.");

        queryable = queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        queryable = sortDir == "desc" ? 
            queryable.OrderByDescending(keySelector) : 
            queryable.OrderBy(keySelector);

        var items = await queryable.ToListAsync(cancellationToken);
        var itemDtos = _mapper.Map<List<TDto>>(items);

        var result = new PagedResponse<TDto>(
            itemDtos,
            totalCount,
            page,
            pageSize,
            pagesCount);

        return result;
    }
}
