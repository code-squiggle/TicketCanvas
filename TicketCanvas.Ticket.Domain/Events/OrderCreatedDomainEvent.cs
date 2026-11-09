using MediatR;
using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Domain.Events;

public record OrderCreatedDomainEvent
(
    Guid OrderId,
    Guid UserId,
    Money TotalAmount,
    string CardToken
) : INotification;