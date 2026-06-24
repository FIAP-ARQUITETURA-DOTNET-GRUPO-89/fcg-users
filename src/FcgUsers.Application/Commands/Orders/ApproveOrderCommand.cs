using MediatR;
using OperationResult;

namespace FcgUsers.Application.Commands.Orders;

public sealed record ApproveOrderCommand(Guid OrderId): IRequest<Result>;
