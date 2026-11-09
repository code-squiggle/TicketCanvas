using TicketCanvas.Common.Domain.Exceptions;

namespace TicketCanvas.Ticket.Domain.ValueObjects;

public record Money(decimal Amount, Currency Currency)
{
    public static Money Create(Currency currency) => new(0, currency);

    public Money Add(Money money)
    {
        if (Currency != money.Currency)
            throw new DomainException("Currency mismatch.");
            
        return this with { Amount = Amount + money.Amount };
    }

    public Money Multiply(int factor)
    {
        return this with { Amount = Amount * factor };
    }
}
