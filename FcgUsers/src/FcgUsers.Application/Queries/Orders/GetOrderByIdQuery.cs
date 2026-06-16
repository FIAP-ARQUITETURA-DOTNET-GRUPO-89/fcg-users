using MediatR;
using FcgUsers.Application.Responses.Orders;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Queries.Orders;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<GetOrderByIdResponse>>, IValidatableRequest
{
    public string? UserId { get; set; }
    public bool IsAdmin { get; set; }
}
