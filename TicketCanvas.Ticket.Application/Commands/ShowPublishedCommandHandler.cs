using MediatR;
using TicketCanvas.Ticket.Application.Repositories;
using TicketCanvas.Ticket.Domain.Aggregates;

namespace TicketCanvas.Ticket.Application.Commands;

public class ShowPublishedCommandHandler : IRequestHandler<ShowPublishedCommand>
{
    private readonly ITicketAllocationRepository _ticketAllocationRepository;
    private readonly IPublisher _publisher;

    public ShowPublishedCommandHandler(
        ITicketAllocationRepository ticketAllocationRepository,
        IPublisher publisher)
    {
        _ticketAllocationRepository = ticketAllocationRepository;
        _publisher = publisher;
    }

    public async Task Handle(ShowPublishedCommand cmd, CancellationToken cancellationToken)
    {
        var ticketAllocations = cmd.TicketTypes.Select(ticketType =>
            TicketAllocation.Create(
                ticketType.Id,
                cmd.ShowId,
                cmd.ShowName,
                ticketType.Name,
                ticketType.Price, 
                ticketType.Currency,
                ticketType.TotalQuantity));
        
        _ticketAllocationRepository.AddRange(ticketAllocations);

        foreach (var ticketAllocation in ticketAllocations)
        {
            foreach (var domainEvent in ticketAllocation.DomainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
            ticketAllocation.ClearDomainEvents();
        }

        await _ticketAllocationRepository.SaveChanges(cancellationToken);
    }
}