using FcgUsers.Application.Commands.Users;
using FluentValidation;

namespace FcgUsers.Application.Validators.Users;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O ID do usuário é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome não pode estar vazio.")
            .MaximumLength(100)
            .WithMessage("O nome não deve exceder 100 caracteres.");

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .WithMessage("A data de nascimento é obrigatória.")
            .LessThan(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("A data de nascimento não pode ser uma data futura.");
    }
}
