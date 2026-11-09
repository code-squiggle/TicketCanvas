using TicketCanvas.Common.Domain.Exceptions;
using TicketCanvas.Common.Domain.Results;
using TicketCanvas.Ticket.Domain.Dtos;
using TicketCanvas.Ticket.Domain.Events;
using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Domain.Aggregates;

public class Order : AggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid IdempotencyKey { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; } = Money.Create(Currency.USD);

    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public static Result<Order> Create(
        Guid userId,
        Guid idempotencyKey,
        IReadOnlyList<OrderItemDto> items,
        string cardToken)
    {
        if (!items.Any())
            throw new DomainException("Order must have a least one item.");

        if (string.IsNullOrEmpty(cardToken))
            throw new DomainException("Card Token must not be empty.");

        var order = new Order
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            IdempotencyKey = idempotencyKey,
            Status = OrderStatus.Pending,
        };

        foreach (var item in items)
        {
            var result = order.AddItem(item);
            if (!result.IsSuccess)
                return Result<Order>.Failure(result.ErrorType);
        }

        order.AddDomainEvent(new OrderCreatedDomainEvent(
            order.Id,
            userId,
            order.TotalAmount,
            cardToken
        ));

        return Result<Order>.Success(order);
    }

    private Result AddItem(OrderItemDto item)
    {
        if (item.TicketAllocation.Price != item.ExpectedPrice)
            return Result.Failure(ErrorType.Conflict, "Price has changed.");
        var result = item.TicketAllocation.Reserve(item.Quantity);

        if (!result.IsSuccess)
            return result;

        TotalAmount = TotalAmount.Add(item.TicketAllocation.Price.Multiply(item.Quantity));

        var orderItem = OrderItem.Create(Id, item.TicketAllocation, item.Quantity);
        _orderItems.Add(orderItem);

        return Result.Success();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be confirmed.");

        Status = OrderStatus.Confirmed;
    }

    public void Fail()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be failed.");

        Status = OrderStatus.Failed;
    }
}

