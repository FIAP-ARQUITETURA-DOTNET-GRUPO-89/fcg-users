using FcgUsers.Application.Commands.Users;
using FluentValidation;

namespace FcgUsers.Application.Validators.Users;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Length(1, 254)
            .EmailAddress()
            .WithMessage("The email format is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .Length(8, 100)
            .WithMessage("Password must be between {MinLength} and {MaxLength} characters.");
    }
}
