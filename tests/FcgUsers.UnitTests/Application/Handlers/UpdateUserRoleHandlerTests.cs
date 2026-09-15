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

public class UpdateUserRoleHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserRoleHandler> _logger;
    private readonly UpdateUserRoleHandler _sut;

    public UpdateUserRoleHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<UpdateUserRoleHandler>>();
        _sut = new UpdateUserRoleHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarRole_Entao_RetornaSucesso()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserRoleCommand(userId, "Admin");
        var user = CreateUser(UserRole.Customer);

        _userRepository.GetByIdAsync(userId).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        user.Role.ShouldBe(UserRole.Admin);
        await _userRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_RoleInexistente_Quando_AtualizarRole_Entao_RetornaErro()
    {
        // Arrange
        var command = new UpdateUserRoleCommand(Guid.NewGuid(), "Invalido");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Exception.ShouldBeOfType<ArgumentException>();
        result.Exception.Message.ShouldBe("Nome de nível de acesso inválido.");
    }

    private static User CreateUser(UserRole role)
        => new(
            "Usuário Teste",
            new DateOnly(1990, 1, 1),
            Email.Create("teste@teste.com"),
            Password.FromHash("hashedpassword123"),
            role
        );
}
