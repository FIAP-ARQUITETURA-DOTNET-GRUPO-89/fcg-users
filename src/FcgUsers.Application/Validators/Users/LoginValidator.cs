using FcgUsers.Application.Commands.Users;
using FluentValidation;

namespace FcgUsers.Application.Validators.Users;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O e-mail é obrigatório.")
            .Length(1, 254)
            .EmailAddress()
            .WithMessage("O formato do e-mail é inválido.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("A senha é obrigatória.")
            .Length(8, 100)
            .WithMessage("A senha deve ter entre {MinLength} e {MaxLength} caracteres.");
    }
}
