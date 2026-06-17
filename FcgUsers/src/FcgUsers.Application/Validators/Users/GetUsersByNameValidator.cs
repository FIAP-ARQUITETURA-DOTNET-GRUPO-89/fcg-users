using FcgUsers.Application.Queries.Users;
using FluentValidation;

namespace FcgUsers.Application.Validators.Users;

public class GetUsersByNameValidator : AbstractValidator<GetUsersByNameQuery>
{
    public GetUsersByNameValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("The name must not exceed 100 characters.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("The maximum allowed page size is 100.");
    }
}
