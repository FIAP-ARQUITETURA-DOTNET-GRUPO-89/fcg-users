using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.ValueObjects;
using Shouldly;

namespace FcgUsers.UnitTests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarUsuario_Entao_CriaComStatusAtivo()
    {
        // Arrange & Act
        var user = CreateValidUser();

        // Assert
        user.Name.ShouldBe("Iago Pachiani");
        user.IsInactive.ShouldBeFalse();
        user.Role.ShouldBe(UserRole.User);
    }

    [Fact]
    public void Dado_NomeVazio_Quando_AtualizarPerfil_Entao_LancaExcecao()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        Action act = () => user.UpdateProfile("", new DateOnly(1990, 1, 1));

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void Dado_DataFutura_Quando_AtualizarPerfil_Entao_LancaExcecao()
    {
        // Arrange
        var user = CreateValidUser();
        var dataFutura = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        // Act
        Action act = () => user.UpdateProfile("Novo Nome", dataFutura);

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void Dado_DadosValidos_Quando_AtualizarPerfil_Entao_AtualizaCampos()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        user.UpdateProfile("Novo Nome", new DateOnly(1995, 5, 5));

        // Assert
        user.Name.ShouldBe("Novo Nome");
        user.BirthDate.ShouldBe(new DateOnly(1995, 5, 5));
    }

    [Fact]
    public void Dado_UsuarioAtivo_Quando_Desativar_Entao_IsInactiveDeveSerTrue()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        user.Deactivate();

        // Assert
        user.IsInactive.ShouldBeTrue();
    }

    [Fact]
    public void Dado_UsuarioAdmin_Quando_VerificarIsAdmin_Entao_RetornaTrue()
    {
        // Arrange
        var user = new User("Admin", new DateOnly(1990, 1, 1), Email.Create("a@a.com"), Password.FromHash("pass"), UserRole.Admin);

        // Act & Assert
        user.IsAdmin().ShouldBeTrue();
    }

    [Fact]
    public void Dado_DataNascimento_Quando_CalcularIdade_Entao_RetornaIdadeCorreta()
    {
        // Arrange
        var nascimento = new DateOnly(1990, 1, 1);
        var user = new User("Teste", nascimento, Email.Create("a@a.com"), Password.FromHash("pass"), UserRole.User);

        // Act
        var age = user.CalculateAge();

        // Assert
        age.ShouldBeGreaterThan(30);
    }

    private static User CreateValidUser()
        => new(
            "Iago Pachiani",
            new DateOnly(1990, 1, 1),
            Email.Create("iago@teste.com"),
            Password.FromHash("SenhaSegura123!"),
            UserRole.User
        );
}
