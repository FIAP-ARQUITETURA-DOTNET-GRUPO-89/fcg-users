using FcgUsers.Application.Queries.Users;
using FcgUsers.Application.Validators;
using FluentValidation.TestHelper;

namespace FcgUsers.UnitTests.Application.Validators.Users;

public class GetUsersByNameValidatorTests
{
    private readonly GetUsersByNameValidator _validator;

    public GetUsersByNameValidatorTests()
    {
        _validator = new GetUsersByNameValidator();
    }

    [Fact]
    public void Dado_QueryValida_Quando_Validar_Entao_NaoDeveRetornarErros()
    {
        // Arrange
        var query = new GetUsersByNameQuery("Teste", 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dado_NomeMuitoLongo_Quando_Validar_Entao_RetornaErro()
    {
        // Arrange
        var query = new GetUsersByNameQuery(new string('a', 101), 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_PaginaInvalida_Quando_Validar_Entao_RetornaErro(int page)
    {
        var query = new GetUsersByNameQuery("Teste", page, 10);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Dado_PageSizeInvalido_Quando_Validar_Entao_RetornaErro(int pageSize)
    {
        var query = new GetUsersByNameQuery("Teste", 1, pageSize);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
