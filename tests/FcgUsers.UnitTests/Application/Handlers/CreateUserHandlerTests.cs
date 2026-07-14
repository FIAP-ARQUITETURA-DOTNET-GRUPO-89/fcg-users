using FcgUsers.Application.Commands;
using FcgUsers.Application.Handlers.Users;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Repositories;
using FgcGames.EventContracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace FcgUsers.UnitTests.Application.Handlers.Users;

public class CreateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateUserHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly CreateUserHandler _sut;

    public CreateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<CreateUserHandler>>();
        _publishEndpoint = Substitute.For<IPublishEndpoint>();

        _sut = new CreateUserHandler(_userRepository, _logger, _publishEndpoint);
    }

    [Fact]
    public async Task Dado_DadosValidos_Quando_CriarUsuario_Entao_RetornaSucessoEPublishEvent()
    {
        // Arrange
        var command = CreateCommand();
        _userRepository.ExistsByEmailAsync(command.Email).Returns(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _userRepository.Received(1).Add(Arg.Any<User>());
        await _userRepository.Received(1).SaveChangesAsync();
        await _publishEndpoint.Received(1).Publish(Arg.Any<UserCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Dado_EmailJaExistente_Quando_CriarUsuario_Entao_RetornaErro()
    {
        // Arrange
        var command = CreateCommand();
        _userRepository.ExistsByEmailAsync(command.Email).Returns(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Exception.ShouldBeOfType<ArgumentException>();
        result.Exception.Message.ShouldBe("Já existe um usuário com esse email");

        _userRepository.DidNotReceive().Add(Arg.Any<User>());
        await _userRepository.DidNotReceive().SaveChangesAsync();
        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<UserCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    private static CreateUserCommand CreateCommand()
        => new(
            Name: "Usuário Teste",
            BirthDate: new DateOnly(1990, 1, 1),
            Email: "teste@teste.com",
            Password: "Password123!",
            Role: "User"
        );
}
