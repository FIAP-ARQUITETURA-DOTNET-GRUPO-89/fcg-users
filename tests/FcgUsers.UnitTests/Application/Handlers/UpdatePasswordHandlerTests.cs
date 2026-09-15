using FcgUsers.Application.Commands;
using FcgUsers.Application.Handlers.Users;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.Repositories;
using FcgUsers.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace FcgUsers.UnitTests.Application.Handlers.Users;

public class UpdatePasswordHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdatePasswordHandler> _logger;
    private readonly UpdatePasswordHandler _sut;

    public UpdatePasswordHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<UpdatePasswordHandler>>();
        _sut = new UpdatePasswordHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarSenha_Entao_RetornaSucesso()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "teste@teste.com";
        var command = new UpdatePasswordCommand(userId, "NovaSenha123", Guid.NewGuid(), email);
        var user = CreateUser(email);

        _userRepository.GetByIdAsync(userId).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Message.ShouldBe("Senha atualizada com sucesso!");
        _userRepository.Received(1).Update(Arg.Any<User>());
        await _userRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_UsuarioNaoEncontrado_Quando_AtualizarSenha_Entao_RetornaErro()
    {
        // Arrange
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "NovaSenha123", Guid.NewGuid(), "email@teste.com");
        _userRepository.GetByIdAsync(command.Id).Returns((User?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Exception.ShouldBeOfType<KeyNotFoundException>();
    }

    [Fact]
    public async Task Dado_EmailDiferenteDoUsuario_Quando_AtualizarSenha_Entao_RetornaErro()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdatePasswordCommand(userId, "NovaSenha123", Guid.NewGuid(), "outro@email.com");
        var user = CreateUser("dono@email.com");

        _userRepository.GetByIdAsync(userId).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Exception.ShouldBeOfType<KeyNotFoundException>();
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
    }

    private static User CreateUser(string email)
        => new(
            "Usuário Teste",
            new DateOnly(1990, 1, 1),
            Email.Create(email),
            Password.FromHash(BCrypt.Net.BCrypt.HashPassword("SenhaAntiga123")),
            UserRole.Customer
        );
}
