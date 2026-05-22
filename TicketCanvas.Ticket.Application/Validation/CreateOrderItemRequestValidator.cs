using FluentValidation;
using TicketCanvas.Ticket.Application.Dtos;

namespace TicketCanvas.Ticket.Application.Validation;

public class CreateOrderItemRequestValidator : AbstractValidator<CreateOrderItemRequest>
{
    public CreateOrderItemRequestValidator()
    {
        RuleFor(r => r.TicketTypeId)
            .NotEmpty();

        RuleFor(r => r.Quantity)
            .GreaterThan(0)
            .LessThan(10);

        RuleFor(r => r.ExpectedPrice)
            .GreaterThan(0);
    }
}