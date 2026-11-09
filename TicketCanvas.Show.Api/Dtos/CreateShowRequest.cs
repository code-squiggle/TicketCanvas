namespace TicketCanvas.Show.Api.Dto;

public record CreateShowRequest(
    string Name,
    string Description,
    Guid VenueId,
    DateTime StartDateTime,
    DateTime EndDateTime,
    List<CreateTicketTypeRequest> TicketTypes);