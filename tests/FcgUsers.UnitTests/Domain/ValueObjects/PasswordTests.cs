using FcgUsers.Domain.ValueObjects;
using Shouldly;

namespace FcgUsers.UnitTests.Domain.ValueObjects;

public class PasswordTests
{
    [Fact]
    public void Dado_HashValido_Quando_Criar_Entao_ArmazenaHashCorretamente()
    {
        // Arrange
        var hash = "hashed_string_123";

        // Act
        var password = Password.FromHash(hash);

        // Assert
        password.Hash.ShouldBe(hash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Dado_HashVazio_Quando_Criar_Entao_LancaExcecao(string hash)
    {
        // Act
        Action act = () => Password.FromHash(hash);

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void Dado_SenhaPuraValida_Quando_Validar_Entao_NaoLancaExcecao()
    {
        // Arrange
        var senhaPura = "Senha123!";

        // Act & Assert
        Should.NotThrow(() => Password.ValidarTextoPuro(senhaPura));
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("1234567890123")]
    public void Dado_SenhaPuraComTamanhoInvalido_Quando_Validar_Entao_LancaExcecao(string senha)
    {
        // Act
        Action act = () => Password.ValidarTextoPuro(senha);

        // Assert
        Should.Throw<ArgumentException>(act)
              .Message.ShouldBe("A senha deve ter entre 8 e 12 caracteres.");
    }
}
