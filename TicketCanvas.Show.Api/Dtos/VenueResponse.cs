namespace TicketCanvas.Show.Api.Dto;

public record VenueResponse(
    Guid Id,
    string Name,
    string Address,
    string City,
    int Capacity);