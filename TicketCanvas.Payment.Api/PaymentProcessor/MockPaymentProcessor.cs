using TicketCanvas.Payment.Api.Dtos;

namespace TicketCanvas.Payment.Api.PaymentProcessor;

public class MockPaymentProcessor : IPaymentProcessor
{
    public async Task<PaymentResult> Charge(PaymentRequest paymentRequest)
    {
        await Task.Delay(1000);

        if (paymentRequest.CardToken == "failure")
            return new PaymentResult(false, Guid.NewGuid().ToString(), "Failure reason");

        return new PaymentResult(true, Guid.NewGuid().ToString(), null);
    }
}