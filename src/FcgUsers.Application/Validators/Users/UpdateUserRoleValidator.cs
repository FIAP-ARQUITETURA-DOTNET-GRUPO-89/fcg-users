using FcgUsers.Application.Commands.Users;
using FluentValidation;

namespace FcgUsers.Application.Validators.Users;

public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O ID do usuário é obrigatório.");

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("O nome do nível de acesso não pode estar vazio.");
    }
}
