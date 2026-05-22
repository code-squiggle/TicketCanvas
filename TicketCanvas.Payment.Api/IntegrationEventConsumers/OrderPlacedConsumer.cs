using AutoMapper;
using MassTransit;
using TicketCanvas.Common.Application.IntegrationEvents;
using TicketCanvas.Payment.Api.Dtos;
using TicketCanvas.Payment.Api.PaymentProcessor;
using TicketCanvas.Payment.Data;
using TicketCanvas.Payment.Data.Models;
using PaymentModel = TicketCanvas.Payment.Data.Models.Payment;

namespace TicketCanvas.Payment.Api.IntegrationEventConsumers;

public class OrderPlacedConsumer : IConsumer<OrderPlaced> 
{
    private readonly PaymentDbContext _dbContext;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IMapper _mapper;

    public OrderPlacedConsumer(
        PaymentDbContext dbContext,
        IPaymentProcessor paymentProcessor,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _paymentProcessor = paymentProcessor;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<OrderPlaced> context)
    {
        var order = context.Message;
        var payment = _mapper.Map<PaymentModel>(order);
        payment.Status = PaymentStatus.Pending;
        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

        var paymentResult = await _paymentProcessor.Charge(
            new PaymentRequest(
                order.Id,
                order.TotalAmount,
                order.CardToken,
                IdempotencyKey: order.Id.ToString()));
        
        payment.TransactionId = paymentResult.TransactionId;
        if (paymentResult.Succeeded)
        {
            payment.Status = PaymentStatus.Completed;
            var paymentCompleted = new PaymentCompleted(order.Id);
            await context.Publish(paymentCompleted);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = paymentResult.FailureReason;
            var paymentFailed = new PaymentFailed(order.Id, paymentResult.FailureReason);
            await context.Publish(paymentFailed);
            await _dbContext.SaveChangesAsync();
        }
    }
}