namespace TicketCanvas.Show.Api.Dto;

public record CreateTicketTypeRequest(
    string Name,
    decimal Price,
    int TotalQuantity);