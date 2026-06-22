using MediatR;
using FcgUsers.Application.Responses.Orders;
using FcgUsers.SharedKernel.Responses;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Queries.Orders;

public record GetOrdersQuery(int Page = 1, int PageSize = 10)
    : IRequest<Result<PagedResponse<GetOrdersResponse>>>, IValidatableRequest
{
    public string? UserId { get; set; }
    public bool IsAdmin { get; set; }
}
