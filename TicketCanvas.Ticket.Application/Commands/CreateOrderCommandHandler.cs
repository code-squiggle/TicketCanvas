using MediatR;
using TicketCanvas.Common.Domain.Results;
using TicketCanvas.Ticket.Application.Repositories;
using TicketCanvas.Ticket.Domain.Aggregates;
using TicketCanvas.Ticket.Domain.Dtos;
using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Application.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITicketAllocationRepository _ticketAllocationRepository;
    private readonly IPublisher _publisher;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        ITicketAllocationRepository ticketAllocationRepository,
        IPublisher publisher)
    {
        _orderRepository = orderRepository;
        _ticketAllocationRepository = ticketAllocationRepository;
        _publisher = publisher;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand cmd, CancellationToken cancellationToken)
    {
        var ticketAllocationIds = cmd.OrderItems.Select(orderItem => orderItem.TicketTypeId).ToList();
        var ticketAllocations = await _ticketAllocationRepository.GetByIds(ticketAllocationIds);

        if (ticketAllocations.Count != ticketAllocationIds.Count)
            throw new Exception("One or more ticket allocations was not found.");

        var ticketAllocationLookup = ticketAllocations.ToDictionary(ticketAllocation => ticketAllocation.Id);
        var orderItems = cmd.OrderItems
            .Select(item => 
                new OrderItemDto(
                    ticketAllocationLookup[item.TicketTypeId], 
                    item.Quantity, 
                    new Money(item.ExpectedPrice, item.Currency)))
            .ToList();

        var result = Order.Create(cmd.UserId, cmd.IdempotencyKey, orderItems, cmd.CardToken);
        
        if (!result.IsSuccess)
            return Result<Guid>.Failure(result.ErrorType, result.ErrorMessage);

        var order = result.Value;

        _orderRepository.Add(order);

        foreach (var domainEvent in order.DomainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        order.ClearDomainEvents();

        await _orderRepository.SaveChanges(cancellationToken);

        return Result<Guid>.Success(order.Id);
    }
}