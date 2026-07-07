using FcgUsers.Application.Handlers.Users;
using FcgUsers.Application.Queries.Users;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.Repositories;
using FcgUsers.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace FcgUsers.UnitTests.Application.Handlers.Users;

public class GetUsersByNameHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUsersByNameHandler> _logger;
    private readonly GetUsersByNameHandler _sut;

    public GetUsersByNameHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<GetUsersByNameHandler>>();
        _sut = new GetUsersByNameHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Dado_NomeValido_Quando_BuscarPorNome_Entao_RetornaListaPaginada()
    {
        // Arrange
        var query = new GetUsersByNameQuery("Teste", 1, 10);
        var users = new List<User> { CreateUser() };
        var totalCount = 1;

        _userRepository
            .GetByNamePagedAsync(query.Name, query.Page, query.PageSize, Arg.Any<CancellationToken>())
            .Returns((users, totalCount));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Items.ShouldNotBeEmpty();
        result.Value.TotalItems.ShouldBe(totalCount);
        result.Value.CurrentPage.ShouldBe(query.Page);

        await _userRepository.Received(1).GetByNamePagedAsync(query.Name, query.Page, query.PageSize, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Dado_NomeVazioOuNulo_Quando_BuscarPorNome_Entao_RetornaListaVaziaSemConsultarRepositorio()
    {
        // Arrange
        var query = new GetUsersByNameQuery(string.Empty, 1, 10);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Value.TotalItems.ShouldBe(0);

        await _userRepository.DidNotReceive().GetByNamePagedAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
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
