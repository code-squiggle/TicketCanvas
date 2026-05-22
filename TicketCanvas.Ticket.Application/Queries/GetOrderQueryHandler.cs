using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Common.Application;
using TicketCanvas.Common.Domain.Results;
using TicketCanvas.Ticket.Application.DbContexts;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Application.Queries;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Result<OrderDetailResponse>>
{
    private readonly ITicketReadContext _ticketReadContext;
    private readonly IMapper _mapper;

    public GetOrderQueryHandler(ITicketReadContext ticketReadContext, IMapper mapper)
    {
        _ticketReadContext = ticketReadContext;
        _mapper = mapper;
    }

    public async Task<Result<OrderDetailResponse>> Handle(GetOrderQuery query, CancellationToken cancellationToken)
    {
        var ordersQueryable = _ticketReadContext.Orders
            .Include(order => order.OrderItems)
            .Where(order => order.Id == query.OrderId);

        if (query.UserRole == UserRole.Customer)
            ordersQueryable = ordersQueryable.Where(order => order.UserId == query.UserId);
        
        var order = await ordersQueryable.FirstOrDefaultAsync(cancellationToken);

        if (order == null)
            return Result<OrderDetailResponse>.Failure(ErrorType.NotFound);

        return Result<OrderDetailResponse>.Success(_mapper.Map<OrderDetailResponse>(order));
    }
}