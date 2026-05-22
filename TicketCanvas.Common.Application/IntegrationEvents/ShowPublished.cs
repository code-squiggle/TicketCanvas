namespace TicketCanvas.Common.Application.IntegrationEvents;

public record ShowPublished
(
    Guid Id,
    string Name,
    List<TicketType> TicketTypes
);