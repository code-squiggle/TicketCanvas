using MediatR;
using TicketCanvas.Ticket.Application.Repositories;
using TicketModel = TicketCanvas.Ticket.Domain.Aggregates.Ticket;

namespace TicketCanvas.Ticket.Application.Commands;

public class PaymentCompletedCommandHandler : IRequestHandler<PaymentCompletedCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IPublisher _publisher;

    public PaymentCompletedCommandHandler(
        IOrderRepository orderRepository,
        ITicketRepository ticketRepository,
        IPublisher publisher)
    {
        _orderRepository = orderRepository;
        _ticketRepository = ticketRepository;
        _publisher = publisher;
    }

    public async Task Handle(PaymentCompletedCommand cmd, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetById(cmd.OrderId);

        if (order == null)
            throw new ApplicationException("Order is not found.");

        order.Confirm();

        var now = DateTime.UtcNow;
        
        var tickets = order.OrderItems
            .SelectMany(orderItem => Enumerable.Range(1, orderItem.Quantity)
                .Select(_ => TicketModel.Create(
                    orderItem.Id,
                    qrCode: Guid.NewGuid().ToString(),
                    issuedAt: now)));

        _ticketRepository.AddRange(tickets);

        foreach (var ticket in tickets)
        {
            foreach (var domainEvent in ticket.DomainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
            ticket.ClearDomainEvents();
        }

        await _ticketRepository.SaveChanges(cancellationToken);
    }
}