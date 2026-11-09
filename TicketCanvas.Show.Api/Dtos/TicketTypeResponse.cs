namespace TicketCanvas.Show.Api.Dto;

public record TicketTypeResponse(
    Guid Id,
    Guid ShowId,
    string Name,
    decimal Price,
    int TotalQuantity);