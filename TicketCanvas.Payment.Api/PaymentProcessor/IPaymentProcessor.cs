using TicketCanvas.Payment.Api.Dtos;

namespace TicketCanvas.Payment.Api.PaymentProcessor;

public interface IPaymentProcessor
{
    Task<PaymentResult> Charge(PaymentRequest paymentRequest);
}
