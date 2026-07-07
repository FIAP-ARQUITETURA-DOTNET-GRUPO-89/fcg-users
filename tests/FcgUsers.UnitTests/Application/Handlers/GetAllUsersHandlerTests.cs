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

public class GetAllUsersHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllUsersHandler> _logger;
    private readonly GetAllUsersHandler _sut;

    public GetAllUsersHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<GetAllUsersHandler>>();
        _sut = new GetAllUsersHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Dado_ConsultaValida_Quando_BuscarUsuarios_Entao_RetornaListaPaginada()
    {
        // Arrange
        var query = new GetAllUsersQuery(1, 10, true);
        var users = new List<User> { CreateUser() };
        var totalCount = 1;

        _userRepository
            .GetPagedAsync(query.Page, query.PageSize, query.Active, Arg.Any<CancellationToken>())
            .Returns((users, totalCount));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Items.ShouldNotBeEmpty();
        result.Value.TotalItems.ShouldBe(totalCount);
        result.Value.CurrentPage.ShouldBe(query.Page);

        await _userRepository.Received(1).GetPagedAsync(query.Page, query.PageSize, query.Active, Arg.Any<CancellationToken>());
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
