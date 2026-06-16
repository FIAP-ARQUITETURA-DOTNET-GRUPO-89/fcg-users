using MediatR;
using FcgUsers.Application.Mappers;
using FcgUsers.Application.Queries.Orders;
using FcgUsers.Application.Responses.Orders;
using FcgUsers.Domain.Repositories.Orders;
using FcgUsers.SharedKernel.Exceptions;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace FcgUsers.Application.Handlers.Orders;

public sealed partial class GetOrderByIdHandler(
    IOrderRepository orderRepository,
    ILogger<GetOrderByIdHandler> logger)
: IRequestHandler<GetOrderByIdQuery, Result<GetOrderByIdResponse>>
{
    public async Task<Result<GetOrderByIdResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
        {
            return new NotFoundException($"Pedido com ID {request.Id} não foi encontrado.");
        }

        if (!request.IsAdmin && order.Customer != request.UserId)
        {
            LogUnauthorizedAccessAttempt(logger, request.Id, request.UserId);

            return new NotFoundException($"Pedido com ID {request.Id} não foi encontrado.");
        }

        return Result.Success(order.ToGetOrderByIdResponse());
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Usuário {UserId} tentou acessar indevidamente o pedido {OrderId} de outro cliente.")]
    private static partial void LogUnauthorizedAccessAttempt(ILogger logger, Guid orderId, string? userId);
}
