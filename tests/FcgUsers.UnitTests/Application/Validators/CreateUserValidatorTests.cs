using FcgUsers.Application.Commands;
using FcgUsers.Application.Validators;
using FluentValidation.TestHelper;

namespace FcgUsers.UnitTests.Application.Validators.Users;

public class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator;

    public CreateUserValidatorTests()
    {
        _validator = new CreateUserValidator();
    }

    [Fact]
    public void Dado_ComandoValido_Quando_Validar_Entao_NaoDeveRetornarErros()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Name
    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("NomeComNumero123")]
    public void Dado_NomeInvalido_Quando_Validar_Entao_RetornaErro(string nome)
    {
        var command = CreateValidCommand() with { Name = nome };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    // BirthDate
    [Fact]
    public void Dado_DataFutura_Quando_Validar_Entao_RetornaErro()
    {
        var command = CreateValidCommand() with { BirthDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)) };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BirthDate);
    }

    // Email
    [Theory]
    [InlineData("emailinvalido")]
    [InlineData("")]
    public void Dado_EmailInvalido_Quando_Validar_Entao_RetornaErro(string email)
    {
        var command = CreateValidCommand() with { Email = email };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    // Password
    [Theory]
    [InlineData("1234567")]
    [InlineData("senhaSemEspecial1")]
    [InlineData("SENHASEMESPECIAL1")]
    public void Dado_SenhaInvalida_Quando_Validar_Entao_RetornaErro(string senha)
    {
        var command = CreateValidCommand() with { Password = senha };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    private static CreateUserCommand CreateValidCommand()
        => new(
            Name: "Usuario Teste",
            BirthDate: new DateOnly(1990, 1, 1),
            Email: "teste@teste.com",
            Password: "Password123!",
            Role: "Customer"
        );
}
