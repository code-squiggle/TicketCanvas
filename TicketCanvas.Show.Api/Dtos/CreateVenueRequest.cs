namespace TicketCanvas.Show.Api.Dto;

public record CreateVenueRequest(
    string Name,
    string Address,
    string City,
    int Capacity);