using FcgUsers.Application.Queries;
using FluentValidation;

namespace FcgUsers.Application.Validators;

public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O ID do usuário é obrigatório.");
    }
}
