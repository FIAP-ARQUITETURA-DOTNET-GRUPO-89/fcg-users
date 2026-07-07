using FcgUsers.Application.Queries.Users;
using FcgUsers.Application.Validators;
using FluentValidation.TestHelper;

namespace FcgUsers.UnitTests.Application.Validators.Users;

public class GetAllUsersValidatorTests
{
    private readonly GetAllUsersValidator _validator;

    public GetAllUsersValidatorTests()
    {
        _validator = new GetAllUsersValidator();
    }

    [Fact]
    public void Dado_QueryValida_Quando_Validar_Entao_NaoDeveRetornarErros()
    {
        // Arrange
        var query = new GetAllUsersQuery(1, 10, true);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_PaginaInvalida_Quando_Validar_Entao_RetornaErro(int page)
    {
        var query = new GetAllUsersQuery(page, 10, true);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Dado_PageSizeInvalido_Quando_Validar_Entao_RetornaErro(int pageSize)
    {
        var query = new GetAllUsersQuery(1, pageSize, true);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
