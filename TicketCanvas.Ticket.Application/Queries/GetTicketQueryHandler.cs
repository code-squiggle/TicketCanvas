using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Common.Application;
using TicketCanvas.Ticket.Application.DbContexts;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Application.Queries;

public class GetTicketQueryHandler : IRequestHandler<GetTicketQuery, TicketDetailResponse?>
{
    private readonly ITicketReadContext _ticketReadContext;
    private readonly IMapper _mapper;

    public GetTicketQueryHandler(ITicketReadContext ticketReadContext, IMapper mapper)
    {
        _ticketReadContext = ticketReadContext;
        _mapper = mapper;
    }

    public async Task<TicketDetailResponse?> Handle(GetTicketQuery query, CancellationToken cancellationToken)
    {
        var ticketsQueryable = _ticketReadContext.Tickets
            .Include(ticket => ticket.OrderItem)
            .ThenInclude(orderItem => orderItem.Order)
            .Where(ticket => ticket.Id == query.TicketId);

        if (query.UserRole == UserRole.Customer)
            ticketsQueryable = ticketsQueryable.Where(ticket => ticket.OrderItem.Order.UserId == query.UserId);
        
        var ticket = await ticketsQueryable.FirstOrDefaultAsync(cancellationToken);

        return _mapper.Map<TicketDetailResponse>(ticket);
    }
}