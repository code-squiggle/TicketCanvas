using MassTransit;
using MediatR;
using TicketCanvas.Common.Application.IntegrationEvents;
using TicketCanvas.Ticket.Application.Commands;

namespace TicketCanvas.Ticket.Infrastructure.IntegrationEventConsumers;

public class PaymentFailedConsumer : IConsumer<PaymentFailed>
{
    private readonly IMediator _mediator;

    public PaymentFailedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<PaymentFailed> context)
    {
        await _mediator.Send(new PaymentFailedCommand(context.Message.OrderId), context.CancellationToken);
    }
}