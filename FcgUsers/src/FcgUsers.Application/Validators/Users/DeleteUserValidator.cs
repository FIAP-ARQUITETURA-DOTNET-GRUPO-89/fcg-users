using FcgUsers.Application.Commands.Users;
using FluentValidation;

namespace FcgUsers.Application.Validators.Users;

public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("The user Id is required and must be a valid identifier.");
    }
}
