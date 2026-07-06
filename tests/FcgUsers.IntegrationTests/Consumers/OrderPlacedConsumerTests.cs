//using System.Net;
//using System.Net.Http.Json;
//using FcgUsers.Application.Commands.Orders;
//using FcgUsers.Application.Responses.Orders;
//using FcgUsers.Domain.Enums;
//using FcgUsers.IntegrationTests.Fixtures;
//using FcgUsers.IntegrationTests.TestHelpers;
//using Microsoft.EntityFrameworkCore;
//using Shouldly;

//namespace FcgUsers.IntegrationTests.Consumers;

//[Collection("IntegrationTests")]
//public class OrderPlacedConsumerTests(IntegrationTestFixture fixture) : IAsyncLifetime
//{
//    private const string OrdersEndpoint = "/api/orders";

//    private readonly IntegrationTestFixture _fixture = fixture;

//    public async ValueTask InitializeAsync()
//        => await _fixture.ResetDatabaseAsync();

//    public ValueTask DisposeAsync()
//        => ValueTask.CompletedTask;

//    [Fact]
//    public async Task Dado_PedidoCriado_Quando_WorkerProcessa_Entao_AprovaPedido()
//    {
//        // Arrange
//        var client = await TestAuthHelper.CreateUserCustomerAsync(_fixture);
//        var request = CreateValidOrderCommand();

//        // Act
//        var response = await client.PostAsJsonAsync(OrdersEndpoint, request, TestContext.Current.CancellationToken);

//        // Assert API
//        response.StatusCode.ShouldBe(HttpStatusCode.Created);
//        var created = await response.ReadContentAsync<CreateOrderResponse>(TestContext.Current.CancellationToken);

//        created.ShouldNotBeNull();
//        created.Id.ShouldNotBe(Guid.Empty);

//        // Assert Worker (Consistência Eventual)
//        var updatedOrder = await WaitUntil.ForAsync(
//            fetchAction: () => _fixture.ExecuteDbContextAsync(async db =>
//                await db.Orders
//                    .AsNoTracking()
//                    .Where(x => x.Id == created.Id)
//                    .Select(x => (OrderStatus?)x.Status)
//                    .FirstOrDefaultAsync()),
//            predicate: status => status == OrderStatus.Approved,
//            timeout: TimeSpan.FromSeconds(30)
//        );

//        updatedOrder.ShouldBe(OrderStatus.Approved);
//    }

//    private static CreateOrderCommand CreateValidOrderCommand()
//        => new(
//            Customer: "Cliente Teste",
//            TotalAmount: 150.00m,
//            Street: "Rua Teste",
//            City: "Lages",
//            State: "SC",
//            Cep: "88500000");
//}
