using MediatR;

namespace TicketCanvas.Ticket.Application.Commands;

public record PaymentCompletedCommand
(
    Guid OrderId
) : IRequest;