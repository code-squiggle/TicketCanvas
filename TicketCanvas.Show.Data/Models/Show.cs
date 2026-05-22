namespace TicketCanvas.Show.Data.Models;

public class Show : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid VenueId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public ShowStatus Status { get; set; }
    public List<TicketType> TicketTypes { get; set; } = [];
    public Venue Venue { get; set; } = new();
}
