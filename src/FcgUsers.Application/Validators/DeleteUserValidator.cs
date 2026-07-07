using FcgUsers.Application.Commands;
using FluentValidation;

namespace FcgUsers.Application.Validators;

public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O ID do usuário é obrigatório e deve ser um identificador válido.");
    }
}
