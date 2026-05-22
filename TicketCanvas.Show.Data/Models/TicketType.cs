namespace TicketCanvas.Show.Data.Models;

public class TicketType : Entity
{
    public Guid ShowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int TotalQuantity { get; set; }
    public Show Show { get; set; } = new();
}
