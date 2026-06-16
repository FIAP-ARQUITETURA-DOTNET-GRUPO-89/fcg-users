using FcgUsers.Domain.Entities;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.Infrastructure.Repositories.Orders;
using FcgUsers.UnitTests.TestHelpers.Factories;
using Shouldly;

namespace FcgUsers.UnitTests.Infra.Repositories.Orders;

public class OrderRepositoryTests
{
    [Fact]
    public async Task Dado_PedidoValido_Quando_Adicionar_Entao_DevePersistir()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new OrderRepository(context);

        var order = CreateOrder("John Doe");

        // Act
        repository.Add(order);
        await repository.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(order.Id, TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result!.Customer.ShouldBe("John Doe");
        result.TotalAmount.ShouldBe(100);
    }

    [Fact]
    public async Task Dado_PedidoExistente_Quando_BuscarPorId_Entao_DeveRetornarPedido()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new OrderRepository(context);

        var order = CreateOrder("Maria");

        context.Orders.Add(order);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetByIdAsync(order.Id, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result!.Customer.ShouldBe("Maria");
    }

    [Fact]
    public async Task Dado_PedidoInexistente_Quando_BuscarPorId_Entao_DeveRetornarNulo()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new OrderRepository(context);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Dado_MultiplosPedidos_Quando_BuscarPaginado_Entao_DeveRetornarPaginaCorreta()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new OrderRepository(context);

        context.Orders.AddRange(
            CreateOrder("Ana"),
            CreateOrder("Bruno"),
            CreateOrder("Carlos"),
            CreateOrder("Daniel")
        );

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(page: 2, pageSize: 2, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        items.Count.ShouldBe(2);
        totalCount.ShouldBe(4);
        items.First().Customer.ShouldBe("Carlos");
    }

    [Fact]
    public async Task Dado_FiltroPorCliente_Quando_BuscarPaginado_Entao_DeveFiltrarCorretamente()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new OrderRepository(context);

        context.Orders.AddRange(
            CreateOrder("Ana"),
            CreateOrder("Bruno"),
            CreateOrder("Bruno"),
            CreateOrder("Carlos")
        );

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(page: 1, pageSize: 10, customerId: "Bruno", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        items.Count.ShouldBe(2);
        totalCount.ShouldBe(2);
        items.All(x => x.Customer == "Bruno").ShouldBeTrue();
    }

    [Fact]
    public async Task Dado_PedidoExistente_Quando_AtualizarESalvar_Entao_DevePersistirAlteracao()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new OrderRepository(context);

        var order = CreateOrder("Original");

        context.Orders.Add(order);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        order.ForceStatus(OrderStatus.Approved);
        await repository.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(order.Id, TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result!.Status.ShouldBe(OrderStatus.Approved);
    }

    private static Order CreateOrder(string customer)
        => new(
            customer,
            100,
            new Address("Street 1", "City", "SP", "88550000"));
}
