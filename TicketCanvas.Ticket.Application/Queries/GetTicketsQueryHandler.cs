using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Common.Application;
using TicketCanvas.Ticket.Application.DbContexts;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Application.Queries;

public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, IEnumerable<TicketSummaryResponse>>
{
    private readonly ITicketReadContext _ticketReadContext;
    private readonly IMapper _mapper;

    public GetTicketsQueryHandler(ITicketReadContext ticketReadContext, IMapper mapper)
    {
        _ticketReadContext = ticketReadContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TicketSummaryResponse>> Handle(GetTicketsQuery query, CancellationToken cancellationToken)
    {
        Guid? userId = query.UserId;
        
        if (query.UserRole == UserRole.Customer)
            userId = query.CurrentUserId;

        var queryableTickets = _ticketReadContext.Tickets
            .Include(ticket => ticket.OrderItem)
            .ThenInclude(orderItem => orderItem.Order)
            .AsQueryable();

        if (userId != null)
            queryableTickets = queryableTickets.Where(ticket => ticket.OrderItem.Order.UserId == userId);

        if (query.OrderId != null)
            queryableTickets = queryableTickets.Where(ticket => ticket.OrderItem.OrderId == query.OrderId);

        queryableTickets = queryableTickets.OrderByDescending(ticket => ticket.CreatedAt);

        queryableTickets = queryableTickets.Take(100);

        var tickets = await queryableTickets.ToListAsync(cancellationToken);

        return _mapper.Map<List<TicketSummaryResponse>>(tickets);
    }
}