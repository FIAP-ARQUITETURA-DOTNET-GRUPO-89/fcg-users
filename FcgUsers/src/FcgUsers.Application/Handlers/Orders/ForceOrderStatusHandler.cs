using MediatR;
using FcgUsers.Application.Commands.Orders;
using FcgUsers.Application.Mappers;
using FcgUsers.Application.Responses.Orders;
using FcgUsers.Domain.Enums;
using FcgUsers.Domain.Repositories.Orders;
using FcgUsers.SharedKernel.Exceptions;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace FcgUsers.Application.Handlers.Orders;

public sealed partial class ForceOrderStatusHandler(
    IOrderRepository orderRepository,
    ILogger<ForceOrderStatusHandler> logger)
: IRequestHandler<ForceOrderStatusCommand, Result<ForceOrderStatusResponse>>
{
    public async Task<Result<ForceOrderStatusResponse>> Handle(ForceOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
        {
            LogOrderNotFound(logger, request.Id, request.UserId);

            return new NotFoundException($"Pedido com ID {request.Id} não foi encontrado.");
        }

        order.ForceStatus(Enum.Parse<OrderStatus>(request.NewStatus, ignoreCase: true));

        await orderRepository.SaveChangesAsync();

        LogManualStatusOverride(logger, request.Id, request.NewStatus, request.UserId);

        return Result.Success(order.ToForceOrderStatusResponse());
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Pedido {OrderId} não encontrado. Executado por: {UserId}")]
    private static partial void LogOrderNotFound( ILogger logger, Guid orderId, string? userId);

    [LoggerMessage(Level = LogLevel.Critical, Message = "CONTINGÊNCIA: Admin {UserId} forçou manualmente o status do pedido {OrderId} para {NewStatus}")]
    private static partial void LogManualStatusOverride(ILogger logger, Guid orderId, string newStatus, string? userId);
}
