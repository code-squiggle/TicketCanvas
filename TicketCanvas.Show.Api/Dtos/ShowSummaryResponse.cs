using TicketCanvas.Show.Data.Models;

namespace TicketCanvas.Show.Api.Dto;

public record ShowSummaryResponse(
    Guid Id,
    string Name,
    DateTime StartDateTime,
    VenueResponse Venue,
    ShowStatus Status);