using FcgUsers.Application.Commands.Users;
using FluentValidation;

namespace FcgUsers.Application.Validators.Users;

public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("The user Id is required.");
        RuleFor(x => x.RoleName).NotEmpty().WithMessage("The role name cannot be empty.");
    }
}
