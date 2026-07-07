using FcgUsers.Application.Commands.Users;
using FcgUsers.Application.Validators;
using FluentValidation.TestHelper;

namespace FcgUsers.UnitTests.Application.Validators.Users;

public class UpdateUserValidatorTests
{
    private readonly UpdateUserValidator _validator;

    public UpdateUserValidatorTests()
    {
        _validator = new UpdateUserValidator();
    }

    [Fact]
    public void Dado_ComandoValido_Quando_Validar_Entao_NaoDeveRetornarErros()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.NewGuid(), "Nome Válido", new DateOnly(1990, 1, 1));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dado_IdVazio_Quando_Validar_Entao_RetornaErro()
    {
        var command = new UpdateUserCommand(Guid.Empty, "Nome", new DateOnly(1990, 1, 1));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.")]
    public void Dado_NomeInvalido_Quando_Validar_Entao_RetornaErro(string nome)
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), nome, new DateOnly(1990, 1, 1));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Dado_DataFutura_Quando_Validar_Entao_RetornaErro()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "Nome", DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BirthDate);
    }
}
