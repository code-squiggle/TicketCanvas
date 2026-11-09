using MediatR;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Application.Commands;

public record ShowPublishedCommand
(
    Guid ShowId,
    string ShowName,
    IReadOnlyList<TicketTypeDto> TicketTypes
) : IRequest;