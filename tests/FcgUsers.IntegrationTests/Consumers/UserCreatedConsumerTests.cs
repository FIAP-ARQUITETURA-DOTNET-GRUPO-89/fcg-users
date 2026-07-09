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
    public async Task Dado_EventoUserCreated_Quando_Publicado_Entao_DeveSerProcessadoPeloConsumer()
    {
        // Arrange
        var harness = _fixture.App.Services.GetRequiredService<ITestHarness>();
        await harness.Start(); // Garante que o harness iniciou

        var userId = Guid.NewGuid();
        var message = new UserCreatedEvent(userId, "Nome Teste", "test@example.com", DateTime.UtcNow);

        // Act
        await harness.Bus.Publish(message);

        // Assert
        // Verifica se foi consumido pelo barramento
        var consumed = await harness.Consumed.Any<UserCreatedEvent>();
        consumed.ShouldBeTrue("O evento UserCreatedEvent não foi consumido pelo barramento.");

        // Verifica o consumidor especificamente
        var consumerHarness = harness.GetConsumerHarness<UserCreatedConsumer>();
        var consumedByConsumer = await consumerHarness.Consumed.Any<UserCreatedEvent>();
        consumedByConsumer.ShouldBeTrue("O UserCreatedConsumer não processou o evento.");
    }
}
