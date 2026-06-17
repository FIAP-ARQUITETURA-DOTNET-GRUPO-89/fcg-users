using FcgUsers.Application.Commands.Users;
using FluentValidation;

namespace FcgUsers.Application.Validators.Users;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("The user Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("The name cannot be empty.")
            .MaximumLength(100)
            .WithMessage("The name must not exceed 100 characters.");

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .WithMessage("Birth date is required.")
            .LessThan(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Birth date cannot be a future date.");
    }
}
