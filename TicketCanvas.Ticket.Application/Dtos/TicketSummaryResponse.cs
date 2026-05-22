namespace TicketCanvas.Ticket.Application.Dtos;

public record TicketSummaryResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid ShowId { get; init; }
    public string ShowName { get; init; } = string.Empty;
    public string TicketTypeName { get; init; } = string.Empty;
    public DateTime IssuedAt { get; init; }
}