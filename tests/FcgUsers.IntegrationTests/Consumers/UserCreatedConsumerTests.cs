using FcgUsers.Domain.Events;
using FcgUsers.IntegrationTests.Fixtures;
using FcgUsers.Worker.Consumers;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace FcgUsers.IntegrationTests.Consumers;

[Collection("IntegrationTests")]
public class UserCreatedConsumerTests(IntegrationTestFixture fixture) : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;

    public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Dado_EventoUserCreated_Quando_Consumido_Entao_DeveProcessarComSucesso()
    {
        // Arrange
        var harness = _fixture.App.Services.GetRequiredService<ITestHarness>();

        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var name = "Nome Teste";

        // Criando o evento com os 4 parâmetros exigidos pelo seu record
        var message = new UserCreatedEvent(
            userId,
            name,
            email,
            DateTime.UtcNow
        );

        // Act
        await harness.Bus.Publish(message);

        // Assert 1: Valida se a mensagem foi entregue ao barramento
        var consumed = await harness.Consumed.Any<UserCreatedEvent>();
        consumed.ShouldBeTrue("O evento UserCreatedEvent não foi consumido pelo barramento.");

        // Assert 2: Valida se o consumidor específico (UserCreatedConsumer) foi invocado
        var consumerHarness = harness.GetConsumerHarness<UserCreatedConsumer>();
        var consumedByConsumer = await consumerHarness.Consumed.Any<UserCreatedEvent>();
        consumedByConsumer.ShouldBeTrue("O UserCreatedConsumer não processou o evento.");
    }
}
