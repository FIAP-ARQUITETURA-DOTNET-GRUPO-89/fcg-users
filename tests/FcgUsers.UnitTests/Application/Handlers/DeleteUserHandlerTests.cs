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

public class DeleteUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DeleteUserHandler> _logger;
    private readonly DeleteUserHandler _sut;

    public DeleteUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<DeleteUserHandler>>();

        _sut = new DeleteUserHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Dado_UsuarioExistente_Quando_ExcluirUsuario_Entao_DesativaUsuarioERetornaSucesso()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);
        var user = CreateUser();

        _userRepository.GetByIdAsync(userId).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        _userRepository.Received(1).Update(Arg.Is<User>(u => u.IsInactive == true));
        await _userRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_UsuarioNaoExistente_Quando_ExcluirUsuario_Entao_RetornaErro()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);

        _userRepository.GetByIdAsync(userId).Returns((User?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Exception.ShouldBeOfType<ArgumentException>();
        result.Exception.Message.ShouldBe("Usuário não encontrado.");

        _userRepository.DidNotReceive().Update(Arg.Any<User>());
        await _userRepository.DidNotReceive().SaveChangesAsync();
    }

    private static User CreateUser()
        => new(
            "Usuário Teste",
            new DateOnly(1990, 1, 1),
            Email.Create("teste@teste.com"),
            Password.FromHash("hashedpassword123"),
            UserRole.User
        );
}
