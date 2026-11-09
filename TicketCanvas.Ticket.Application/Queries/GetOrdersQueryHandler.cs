using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Common.Application;
using TicketCanvas.Ticket.Application.DbContexts;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Application.Queries;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderSummaryResponse>>
{
    private readonly ITicketReadContext _ticketReadContext;
    private readonly IMapper _mapper;

    public GetOrdersQueryHandler(ITicketReadContext ticketReadContext, IMapper mapper)
    {
        _ticketReadContext = ticketReadContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderSummaryResponse>> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        Guid? userId = query.UserId;
        
        if (query.UserRole == UserRole.Customer)
            userId = query.CurrentUserId;

        var queryableOrders = _ticketReadContext.Orders.AsQueryable();

        if (userId != null)
            queryableOrders = queryableOrders.Where(order => order.UserId == userId);

        queryableOrders = queryableOrders.OrderByDescending(order => order.CreatedAt);

        queryableOrders = queryableOrders.Take(100);

        var orders = await queryableOrders.ToListAsync(cancellationToken);

        return _mapper.Map<List<OrderSummaryResponse>>(orders);
    }
}