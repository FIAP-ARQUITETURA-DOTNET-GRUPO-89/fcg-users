using FcgUsers.Application.Commands.Orders;
using FcgUsers.Application.Handlers.Orders;
using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Repositories.Orders;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace FcgUsers.UnitTests.Application.Handlers.Orders;

public class CreateOrderHandlerTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CreateOrderHandler> _logger;
    private readonly CreateOrderHandler _sut;

    public CreateOrderHandlerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _logger = Substitute.For<ILogger<CreateOrderHandler>>();

        _sut = new CreateOrderHandler(_orderRepository, _logger);
    }

    [Fact]
    public async Task Dado_ComandoValido_Quando_CriarPedido_Entao_PersisteERetornaResposta()
    {
        // Arrange
        var command = CreateCommand();

        _orderRepository.SaveChangesAsync()
            .Returns(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldNotBe(Guid.Empty);

        _orderRepository.Received(1)
            .Add(Arg.Is<Order>(o =>
                o.Customer == command.Customer &&
                o.TotalAmount == command.TotalAmount &&
                o.DeliveryAddress.Street == command.Street &&
                o.DeliveryAddress.City == command.City &&
                o.DeliveryAddress.State.Equals(command.State, StringComparison.InvariantCultureIgnoreCase) &&
                o.DeliveryAddress.Cep == "88550000"));

        await _orderRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_ErroAoPersistir_Quando_CriarPedido_Entao_PropagaExcecao()
    {
        // Arrange
        var command = CreateCommand();

        _orderRepository
            .SaveChangesAsync()
            .Returns(Task.FromException<int>(new InvalidOperationException("Database error")));

        // Act
        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            _sut.Handle(command, CancellationToken.None));

        // Assert
        exception.Message.ShouldBe("Database error");

        _orderRepository.Received(1)
            .Add(Arg.Is<Order>(o =>
                o.Customer == command.Customer &&
                o.TotalAmount == command.TotalAmount &&
                o.DeliveryAddress.Street == command.Street &&
                o.DeliveryAddress.City == command.City &&
                o.DeliveryAddress.State.Equals(command.State, StringComparison.InvariantCultureIgnoreCase) &&
                o.DeliveryAddress.Cep == "88550000"));

        await _orderRepository.Received(1).SaveChangesAsync();
    }


    private static CreateOrderCommand CreateCommand()
        => new(
            Customer: "John Doe",
            TotalAmount: 150.75m,
            Street: "Main Street",
            City: "São Paulo",
            State: "sp",
            Cep: "88550-000");
}
