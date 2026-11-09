using MediatR;

namespace TicketCanvas.Ticket.Application.Commands;

public record PaymentFailedCommand
(
    Guid OrderId
) : IRequest;