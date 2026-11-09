using MediatR;
using TicketCanvas.Ticket.Application.Repositories;

namespace TicketCanvas.Ticket.Application.Commands;

public class PaymentFailedCommandHandler : IRequestHandler<PaymentFailedCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITicketAllocationRepository _ticketAllocationRepository;
    private readonly IPublisher _publisher;

    public PaymentFailedCommandHandler(
        IOrderRepository orderRepository,
        ITicketAllocationRepository ticketAllocationRepository,
        IPublisher publisher)
    {
        _orderRepository = orderRepository;
        _ticketAllocationRepository = ticketAllocationRepository;
        _publisher = publisher;
    }

    public async Task Handle(PaymentFailedCommand cmd, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetById(cmd.OrderId);

        if (order == null)
            throw new ApplicationException("Order was not found.");

        var orderItemsLookup = order.OrderItems.ToDictionary(orderItem => orderItem.TicketTypeId);

        var ticketTypeIds = order.OrderItems.Select(orderItem => orderItem.TicketTypeId).ToList();

        var ticketAllocations = await _ticketAllocationRepository.GetByIds(ticketTypeIds);

        if (ticketAllocations.Count != ticketTypeIds.Count)
            throw new ApplicationException("One or more TicketAllocations were not found.");

        foreach (var ticketAllocation in ticketAllocations)
        {
            var orderItem = orderItemsLookup[ticketAllocation.Id];
            ticketAllocation.Restore(orderItem.Quantity);
        }

        order.Fail();

        foreach (var ticketAllocation in ticketAllocations)
        {
            foreach (var domainEvent in ticketAllocation.DomainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
            ticketAllocation.ClearDomainEvents();
        }
 
        foreach (var domainEvent in order.DomainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
        order.ClearDomainEvents();

        await _ticketAllocationRepository.SaveChanges(cancellationToken);
    }
}