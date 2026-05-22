using MassTransit;
using MediatR;
using TicketCanvas.Common.Application.IntegrationEvents;
using TicketCanvas.Ticket.Domain.Events;

namespace TicketCanvas.Ticket.Application.DomainEventHandlers;

public class OrderCreatedDomainEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderCreatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new OrderPlaced(
            notification.OrderId,
            notification.UserId,
            notification.TotalAmount.Amount,
            notification.TotalAmount.Currency.ToString(),
            notification.CardToken
        ), cancellationToken);
    }
}