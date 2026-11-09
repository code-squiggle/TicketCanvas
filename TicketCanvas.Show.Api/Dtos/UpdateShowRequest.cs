namespace TicketCanvas.Show.Api.Dto;

public record UpdateShowRequest(
    string Name,
    string Description,
    Guid VenueId,
    DateTime StartDateTime,
    DateTime EndDateTime);