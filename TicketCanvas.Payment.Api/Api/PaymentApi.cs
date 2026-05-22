using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Common.Application;
using TicketCanvas.Common.Application.Dtos;
using TicketCanvas.Payment.Api.Dtos;
using TicketCanvas.Payment.Data;
using PaymentModel = TicketCanvas.Payment.Data.Models.Payment;

namespace TicketCanvas.Payment.Api.Api;

public static class PaymentApi
{
    public static IEndpointRouteBuilder MapPaymentApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("payments");
        api.MapPost("/tokenize", GetCardToken).RequireAuthorization().WithName("GetCardToken");
        api.MapGet("/", GetPayments).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("GetPayments");
        return app;
    }

    public static async Task<Ok<string>> GetCardToken(
        CardDetails cardDetails,
        CancellationToken cancellationToken)
    {
        string cardToken = Guid.NewGuid().ToString();

        return TypedResults.Ok(cardToken);
    }

    public static async Task<Ok<PagedResponse<PaymentResponse>>> GetPayments(
        [AsParameters] GetPaymentsRequest request,
        PaymentDbContext dbContext,
        [FromServices] IPagingHelper pagingHelper,
        CancellationToken cancellationToken)
    {
        var queryablePayments = dbContext.Payments.AsQueryable();

        var sortDictionary = new Dictionary<string, Expression<Func<PaymentModel, object?>>>
        {
            ["failureReason"] = payment => payment.FailureReason,
            ["status"] = payment => payment.Status,
            ["createdAt"] = payment => payment.CreatedAt,
        };

        var result = await pagingHelper.GetPagedItems<PaymentModel, PaymentResponse>(
            request,
            queryablePayments,
            sortDictionary,
            "createdAt",
            "desc",
            cancellationToken);

        return TypedResults.Ok(result);
    }
}