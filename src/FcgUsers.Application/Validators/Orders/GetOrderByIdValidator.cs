using FluentValidation;
using FcgUsers.Application.Queries.Orders;
using FcgUsers.SharedKernel.Validators;

namespace FcgUsers.Application.Validators.Orders;

public class GetOrderByIdValidator : AbstractValidator<GetOrderByIdQuery>, IValidatableRequest
{
    public GetOrderByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
