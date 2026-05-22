using MassTransit;
using MediatR;
using TicketCanvas.Common.Application.IntegrationEvents;
using TicketCanvas.Ticket.Application.Commands;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Infrastructure.IntegrationEventConsumers;

public class ShowPublishedConsumer : IConsumer<ShowPublished> 
{
    private readonly IMediator _mediator;

    public ShowPublishedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ShowPublished> context)
    {
        await _mediator.Send(new ShowPublishedCommand(
            context.Message.Id,
            context.Message.Name,
            context.Message.TicketTypes.Select(ticketType =>
                new TicketTypeDto(
                    ticketType.Id,
                    ticketType.Name,
                    ticketType.Price, 
                    ticketType.Currency,
                    ticketType.TotalQuantity))
                .ToList().AsReadOnly()
        ), context.CancellationToken);
    }
}