using TicketCanvas.Common.Domain.Exceptions;
using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Domain.Aggregates;

public class OrderItem : Entity
{
    public Guid OrderId { get; private set; }
    public Guid ShowId { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public string ShowName { get; private set; } = string.Empty;
    public string TicketTypeName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = Money.Create(Currency.USD);

    private OrderItem() { }

    public static OrderItem Create(Guid orderId, TicketAllocation ticketAllocation, int quantity)
    {
        if (ticketAllocation == null)
            throw new DomainException("Ticket Allocation must not be null.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be positive.");

        var orderItem = new OrderItem
        {
            Id = Guid.CreateVersion7(),
            OrderId = orderId,
            ShowId = ticketAllocation.ShowId,
            TicketTypeId = ticketAllocation.Id,
            ShowName = ticketAllocation.ShowName,
            TicketTypeName = ticketAllocation.TicketTypeName,
            Quantity = quantity,
            UnitPrice = ticketAllocation.Price
        };
        return orderItem;
    }
}
