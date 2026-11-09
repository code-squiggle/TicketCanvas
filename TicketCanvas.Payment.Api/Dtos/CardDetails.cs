namespace TicketCanvas.Payment.Api.Dtos;

public record CardDetails(
    string CardNumber,
    string CardHolder,
    string CardCode
);