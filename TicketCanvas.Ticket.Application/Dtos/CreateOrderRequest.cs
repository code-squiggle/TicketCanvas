namespace TicketCanvas.Ticket.Application.Dtos;

public record CreateOrderRequest
(
    List<CreateOrderItemRequest> OrderItems,
    string CardToken
);