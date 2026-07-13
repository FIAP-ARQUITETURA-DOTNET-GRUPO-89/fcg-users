using FcgUsers.Application.Queries;
using FluentValidation;

namespace FcgUsers.Application.Validators;

public class GetAllUsersValidator : AbstractValidator<GetAllUsersQuery>
{
    public GetAllUsersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("O tamanho da página deve ser maior que 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("O tamanho máximo permitido para a página é 100.");
    }
}
