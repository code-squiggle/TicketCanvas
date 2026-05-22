namespace TicketCanvas.Ticket.Application.Dtos;

public record TicketDetailResponse(
    Guid Id,
    Guid ShowId,
    string ShowName,
    string TicketTypeName,
    string QRCode,
    DateTime IssuedAt);