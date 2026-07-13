using FcgUsers.Domain.ValueObjects;
using Shouldly;

namespace FcgUsers.UnitTests.Domain.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Dado_EmailValido_Quando_Criar_Entao_CriaNormalizado()
    {
        // Arrange
        var address = "  USUARIO@Teste.Com  ";

        // Act
        var email = Email.Create(address);

        // Assert
        email.Address.ShouldBe("usuario@teste.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("emailinvalido")] // Não tem @
    [InlineData("usuario@")]         // Termina com @
    [InlineData("@teste.com")]    // Começa com @
    public void Dado_EmailInvalido_Quando_Criar_Entao_LancaExcecao(string address)
    {
        // Act & Assert
        // A forma correta de capturar a exceção com Shouldly para um método estático:
        Should.Throw<ArgumentException>(() => Email.Create(address));
    }
}
