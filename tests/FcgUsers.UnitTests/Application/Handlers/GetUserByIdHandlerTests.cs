using FcgUsers.Application.Handlers.Users;
using FcgUsers.Application.Queries;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.Repositories;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.SharedKernel.Exceptions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace FcgUsers.UnitTests.Application.Handlers.Users;

public class GetUserByIdHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByIdHandler> _logger;
    private readonly GetUserByIdHandler _sut;

    public GetUserByIdHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<GetUserByIdHandler>>();
        _sut = new GetUserByIdHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Dado_UsuarioExistente_Quando_BuscarPorId_Entao_RetornaDadosDoUsuario()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);
        var user = CreateUser();

        _userRepository.GetByIdAsync(userId).Returns(user);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(user.Id);
        result.Value.Name.ShouldBe(user.Name);

        await _userRepository.Received(1).GetByIdAsync(userId);
    }

    [Fact]
    public async Task Dado_UsuarioNaoExistente_Quando_BuscarPorId_Entao_RetornaErro()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);

        _userRepository.GetByIdAsync(userId).Returns((User?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Exception.ShouldBeOfType<NotFoundException>();
        result.Exception.Message.ShouldBe($"Usuário com o ID {userId} não foi encontrado.");

        await _userRepository.Received(1).GetByIdAsync(userId);
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
