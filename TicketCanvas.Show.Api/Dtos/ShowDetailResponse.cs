using TicketCanvas.Show.Data.Models;

namespace TicketCanvas.Show.Api.Dto;

public record ShowDetailResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime StartDateTime,
    DateTime EndDateTime,
    VenueResponse Venue,
    ShowStatus Status,
    List<TicketTypeResponse> TicketTypes);