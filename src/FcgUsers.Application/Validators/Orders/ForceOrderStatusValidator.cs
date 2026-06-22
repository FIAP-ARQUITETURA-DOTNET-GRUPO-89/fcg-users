using FluentValidation;
using FcgUsers.Application.Commands.Orders;
using FcgUsers.Domain.Enums;

namespace FcgUsers.Application.Validators.Orders;

public class ForceOrderStatusValidator : AbstractValidator<ForceOrderStatusCommand>
{
    public ForceOrderStatusValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.NewStatus)
            .NotEmpty()
            .Must(x => Enum.TryParse<OrderStatus>(x, true, out _))
            .WithMessage($"Status inválido. Valores permitidos: {string.Join(", ", Enum.GetNames<OrderStatus>())}.");
    }
}
