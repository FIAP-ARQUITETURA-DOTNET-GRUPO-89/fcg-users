using FcgUsers.Application.Commands.Users;
using FluentValidation;

namespace FcgUsers.Application.Validators.Users;

public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("The user Id is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("The password cannot be empty.");
    }
}
