using MassTransit;
using MediatR;
using TicketCanvas.Common.Application.IntegrationEvents;
using TicketCanvas.Ticket.Application.Commands;

namespace TicketCanvas.Ticket.Infrastructure.IntegrationEventConsumers;

public class PaymentCompletedConsumer : IConsumer<PaymentCompleted>
{
    private readonly IMediator _mediator;

    public PaymentCompletedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<PaymentCompleted> context)
    {
        await _mediator.Send(new PaymentCompletedCommand(context.Message.OrderId), context.CancellationToken);
    }
}