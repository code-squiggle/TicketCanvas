using TicketCanvas.Ticket.Domain.ValueObjects;

namespace TicketCanvas.Ticket.Application.Dtos;

public record OrderItemResponse(
    Guid Id,
    Guid ShowId,
    Guid TicketTypeId,
    string ShowName,
    string TicketTypeName,
    int Quantity,
    decimal UnitPrice,
    Currency Currency);