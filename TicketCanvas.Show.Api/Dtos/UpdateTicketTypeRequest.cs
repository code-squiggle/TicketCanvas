namespace TicketCanvas.Show.Api.Dto;

public record UpdateTicketTypeRequest(
    string Name,
    decimal Price,
    int TotalQuantity);