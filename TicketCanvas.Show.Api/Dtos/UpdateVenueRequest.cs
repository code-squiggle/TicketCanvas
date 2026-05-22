namespace TicketCanvas.Show.Api.Dto;

public record UpdateVenueRequest(
    string Name,
    string Address,
    string City,
    int Capacity);