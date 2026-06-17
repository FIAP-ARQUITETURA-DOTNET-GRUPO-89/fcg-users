using FcgUsers.Application.Queries.Users;
using FluentValidation;

namespace FcgUsers.Application.Validators.Users;

public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("The user Id is required.");
    }
}
