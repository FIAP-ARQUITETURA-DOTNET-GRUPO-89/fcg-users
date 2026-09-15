using FcgUsers.Application.Commands;
using FcgUsers.Application.Handlers.Users;
using FcgUsers.Application.Interfaces;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.Repositories;
using FcgUsers.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace FcgUsers.UnitTests.Application.Handlers.Users;

public class LoginHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginHandler> _logger;
    private readonly LoginHandler _sut;

    public LoginHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _tokenService = Substitute.For<ITokenService>();
        _logger = Substitute.For<ILogger<LoginHandler>>();
        _sut = new LoginHandler(_userRepository, _tokenService, _logger);
    }

    [Fact]
    public async Task Dado_CredenciaisValidas_Quando_RealizarLogin_Entao_RetornaToken()
    {
        // Arrange
        var passwordRaw = "Password123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(passwordRaw);
        var email = "teste@teste.com";
        var command = new LoginCommand(email, passwordRaw);
        var user = CreateUser(email, passwordHash);

        _userRepository.GetByEmailAsync(email).Returns(user);

        _tokenService.GenerateJwtToken(user).Returns("fake-jwt-token");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Token.ShouldBe("fake-jwt-token");
    }

    [Fact]
    public async Task Dado_UsuarioInativo_Quando_RealizarLogin_Entao_RetornaUnauthorized()
    {
        // Arrange
        var email = "inativo@teste.com";
        var command = new LoginCommand(email, "Password123!");
        var user = CreateUser(email, BCrypt.Net.BCrypt.HashPassword("Password123!"));
        user.Deactivate();

        _userRepository.GetByEmailAsync(email).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Exception.ShouldBeOfType<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Dado_SenhaIncorreta_Quando_RealizarLogin_Entao_RetornaUnauthorized()
    {
        // Arrange
        var email = "teste@teste.com";
        var command = new LoginCommand(email, "SenhaErrada!");
        var user = CreateUser(email, BCrypt.Net.BCrypt.HashPassword("Password123!"));

        _userRepository.GetByEmailAsync(email).Returns(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Exception.ShouldBeOfType<UnauthorizedAccessException>();
    }

    private static User CreateUser(string email, string passwordHash)
        => new(
            "Usuário Teste",
            new DateOnly(1990, 1, 1),
            Email.Create(email),
            Password.FromHash(passwordHash),
            UserRole.Customer
        );
}
