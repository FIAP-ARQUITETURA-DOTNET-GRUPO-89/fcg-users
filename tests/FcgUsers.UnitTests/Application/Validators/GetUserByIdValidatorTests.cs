using FcgUsers.Application.Queries;
using FcgUsers.Application.Validators;
using FluentValidation.TestHelper;

namespace FcgUsers.UnitTests.Application.Validators.Users;

public class GetUserByIdValidatorTests
{
    private readonly GetUserByIdValidator _validator;

    public GetUserByIdValidatorTests()
    {
        _validator = new GetUserByIdValidator();
    }

    [Fact]
    public void Dado_IdValido_Quando_Validar_Entao_NaoDeveRetornarErros()
    {
        // Arrange
        var query = new GetUserByIdQuery(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dado_IdVazio_Quando_Validar_Entao_RetornaErro()
    {
        // Arrange
        var query = new GetUserByIdQuery(Guid.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("O ID do usuário é obrigatório.");
    }
}
