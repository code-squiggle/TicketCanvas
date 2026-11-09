using FluentValidation;
using TicketCanvas.Ticket.Application.Commands;

namespace TicketCanvas.Ticket.Application.Validation;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(r => r.CardToken)
            .NotEmpty()
            .MaximumLength(36);

        RuleFor(r => r.OrderItems)
            .NotEmpty();
    }
}