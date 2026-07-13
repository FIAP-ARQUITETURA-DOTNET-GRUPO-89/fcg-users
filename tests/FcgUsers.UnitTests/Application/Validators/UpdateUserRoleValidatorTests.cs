using FcgUsers.Application.Commands;
using FcgUsers.Application.Validators;
using FluentValidation.TestHelper;

namespace FcgUsers.UnitTests.Application.Validators.Users;

public class UpdateUserRoleValidatorTests
{
    private readonly UpdateUserRoleValidator _validator;

    public UpdateUserRoleValidatorTests()
    {
        _validator = new UpdateUserRoleValidator();
    }

    [Fact]
    public void Dado_ComandoValido_Quando_Validar_Entao_NaoDeveRetornarErros()
    {
        // Arrange
        var command = new UpdateUserRoleCommand(Guid.NewGuid(), "Admin");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dado_IdVazio_Quando_Validar_Entao_RetornaErro()
    {
        // Arrange
        var command = new UpdateUserRoleCommand(Guid.Empty, "Admin");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("O ID do usuário é obrigatório.");
    }

    [Fact]
    public void Dado_RoleNameVazio_Quando_Validar_Entao_RetornaErro()
    {
        // Arrange
        var command = new UpdateUserRoleCommand(Guid.NewGuid(), "");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoleName)
              .WithErrorMessage("O nome do nível de acesso não pode estar vazio.");
    }
}
