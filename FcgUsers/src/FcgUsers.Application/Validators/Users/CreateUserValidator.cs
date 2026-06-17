using FluentValidation;
using FcgUsers.Application.Commands.Users;

namespace FcgUsers.Application.Validators.Users;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(a => a.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(2).WithMessage("O nome deve ter pelo menos 2 caracteres.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("O nome deve conter apenas letras.");

        RuleFor(a => a.BirthDate)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .Must(d => d < DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("A data de nascimento deve ser no passado.");

        RuleFor(a => a.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .Length(1, 254).WithMessage("O email deve ter entre 1 e 254 caracteres.")
            .EmailAddress().WithMessage("O formato do email é inválido.");

        RuleFor(a => a.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .Length(8, 12).WithMessage("A senha deve ter entre 8 e 12 caracteres.")
            .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
            .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número.")
            .Matches(@"[!?\*\.@#$%&]").WithMessage("A senha deve conter pelo menos um caractere especial. Exemplos permitidos: ! ? * . @ # $ % &");
    }
}
