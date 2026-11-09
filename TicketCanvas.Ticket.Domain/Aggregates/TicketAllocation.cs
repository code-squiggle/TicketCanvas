using TicketCanvas.Common.Domain.Exceptions;
using TicketCanvas.Common.Domain.Results;
using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Domain.Aggregates;

public class TicketAllocation : AggregateRoot
{
    public Guid ShowId { get; private set; }
    public string ShowName { get; private set; } = string.Empty;
    public string TicketTypeName { get; private set; } = string.Empty;
    public Money Price { get; private set; } = Money.Create(Currency.USD);
    public int TotalQuantity { get; private set; }
    public int AvailableQuantity { get; private set; }

    public static TicketAllocation Create(
        Guid id,
        Guid showId,
        string showName,
        string ticketTypeName,
        decimal price,
        string currency,
        int quantity)
    {
        if (string.IsNullOrEmpty(showName))
            throw new DomainException("Show name must not be empty.");

        if (string.IsNullOrEmpty(ticketTypeName))
            throw new DomainException("Ticket Type name must not be empty.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be positive.");

        if (price <= 0)
            throw new DomainException("Price must be positive.");

        if (!Enum.TryParse(currency, out Currency parsedCurrency))
            throw new DomainException("Invalid currency.");

        var priceMoney = new Money(price, parsedCurrency);

        var ticketAllocation = new TicketAllocation
        {
            Id = id,
            ShowId = showId,
            ShowName = showName,
            TicketTypeName = ticketTypeName,
            Price = priceMoney,
            TotalQuantity = quantity,
            AvailableQuantity = quantity
        };

        return ticketAllocation;
    }

    public void Restore(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive.");

        if (AvailableQuantity + quantity > TotalQuantity)
            throw new DomainException("Insufficient capacity.");

        AvailableQuantity += quantity;
    }

    public Result Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive.");

        if (AvailableQuantity - quantity < 0)
            return Result.Failure(ErrorType.Conflict, "Insufficient capacity.");

        AvailableQuantity -= quantity;

        return Result.Success();
    }
}
