using FcgUsers.Application.Commands.Users;
using FcgUsers.Application.Validators;
using FluentValidation.TestHelper;

namespace FcgUsers.UnitTests.Application.Validators.Users;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator;

    public LoginValidatorTests()
    {
        _validator = new LoginValidator();
    }

    [Fact]
    public void Dado_ComandoValido_Quando_Validar_Entao_NaoDeveRetornarErros()
    {
        // Arrange
        var command = new LoginCommand("teste@teste.com", "Password123!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("emailinvalido")]
    [InlineData("")]
    public void Dado_EmailInvalido_Quando_Validar_Entao_RetornaErro(string email)
    {
        var command = new LoginCommand(email, "Password123!");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("")]
    public void Dado_SenhaInvalida_Quando_Validar_Entao_RetornaErro(string senha)
    {
        var command = new LoginCommand("teste@teste.com", senha);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
