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

public class UpdateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserHandler> _logger;
    private readonly UpdateUserHandler _sut;

    public UpdateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<UpdateUserHandler>>();
        _sut = new UpdateUserHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarPerfil_Entao_RetornaSucesso()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand(userId, "Novo Nome", new DateOnly(1995, 5, 5));
        var user = CreateUser();

        _userRepository.GetByIdAsync(userId).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Name.ShouldBe("Novo Nome");
        result.Value.BirthDate.ShouldBe(new DateOnly(1995, 5, 5));

        _userRepository.Received(1).Update(Arg.Any<User>());
        await _userRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_UsuarioNaoEncontrado_Quando_AtualizarPerfil_Entao_RetornaErro()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.NewGuid(), "Nome", new DateOnly(1995, 5, 5));
        _userRepository.GetByIdAsync(command.Id).Returns((User?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Exception.ShouldBeOfType<KeyNotFoundException>();
    }

    [Fact]
    public async Task Dado_DadosInvalidosDeDominio_Quando_AtualizarPerfil_Entao_RetornaErro()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand(userId, "", new DateOnly(2030, 1, 1));
        var user = CreateUser();

        _userRepository.GetByIdAsync(userId).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Exception.ShouldBeOfType<ArgumentException>();
        _userRepository.DidNotReceive().Update(Arg.Any<User>());
    }

    private static User CreateUser()
        => new(
            "Usuário Antigo",
            new DateOnly(1990, 1, 1),
            Email.Create("teste@teste.com"),
            Password.FromHash("hashedpassword123"),
            UserRole.User
        );
}
