using FcgUsers.Application.Commands;
using FcgUsers.Application.Validators;
using FluentValidation.TestHelper;

namespace FcgUsers.UnitTests.Application.Validators.Users;

public class UpdatePasswordValidatorTests
{
    private readonly UpdatePasswordValidator _validator;

    public UpdatePasswordValidatorTests()
    {
        _validator = new UpdatePasswordValidator();
    }

    [Fact]
    public void Dado_ComandoValido_Quando_Validar_Entao_NaoDeveRetornarErros()
    {
        // Arrange
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "NovaSenha123!", Guid.NewGuid(), "teste@teste.com");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dado_IdVazio_Quando_Validar_Entao_RetornaErro()
    {
        // Arrange
        var command = new UpdatePasswordCommand(Guid.Empty, "NovaSenha123!", Guid.NewGuid(), "teste@teste.com");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("O ID do usuário é obrigatório.");
    }

    [Fact]
    public void Dado_SenhaVazia_Quando_Validar_Entao_RetornaErro()
    {
        // Arrange
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "", Guid.NewGuid(), "teste@teste.com");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
              .WithErrorMessage("A senha não pode estar vazia.");
    }
}
