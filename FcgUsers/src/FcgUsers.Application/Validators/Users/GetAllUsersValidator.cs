using FcgUsers.Application.Queries.Users;
using FluentValidation;

namespace FcgUsers.Application.Validators.Users;

public class GetAllUsersValidator : AbstractValidator<GetAllUsersQuery>
{
    public GetAllUsersValidator()
    {
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
