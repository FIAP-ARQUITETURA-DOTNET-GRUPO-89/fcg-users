using FcgUsers.Application.Commands;
using FcgUsers.Application.Validators;
using FluentValidation.TestHelper;

namespace FcgUsers.UnitTests.Application.Validators.Users;

public class DeleteUserValidatorTests
{
    private readonly DeleteUserValidator _validator;

    public DeleteUserValidatorTests()
    {
        _validator = new DeleteUserValidator();
    }

    [Fact]
    public void Dado_IdValido_Quando_Validar_Entao_NaoDeveRetornarErros()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dado_IdVazio_Quando_Validar_Entao_RetornaErro()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("O ID do usuário é obrigatório e deve ser um identificador válido.");
    }
}
