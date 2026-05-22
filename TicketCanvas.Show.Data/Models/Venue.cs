namespace TicketCanvas.Show.Data.Models;

public class Venue : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public List<Show> Shows { get; set; } = [];
}
