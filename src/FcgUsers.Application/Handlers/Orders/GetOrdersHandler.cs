using MediatR;
using FcgUsers.Application.Mappers;
using FcgUsers.Application.Queries.Orders;
using FcgUsers.Application.Responses.Orders;
using FcgUsers.Domain.Repositories.Orders;
using FcgUsers.SharedKernel.Responses;
using OperationResult;

namespace FcgUsers.Application.Handlers.Orders;

public sealed class GetOrdersHandler(
    IOrderRepository orderRepository)
: IRequestHandler<GetOrdersQuery, Result<PagedResponse<GetOrdersResponse>>>
{
    public async Task<Result<PagedResponse<GetOrdersResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var customerId = request.IsAdmin
            ? null
            : request.UserId;

        var (items, totalCount) = await orderRepository.GetPagedAsNoTrackingAsync(
            request.Page,
            request.PageSize,
            customerId,
            cancellationToken);

        var pagedResponse = new PagedResponse<GetOrdersResponse>(items.ToGetOrdersResponseList(), totalCount, request.Page, request.PageSize);

        return Result.Success(pagedResponse);
    }
}
