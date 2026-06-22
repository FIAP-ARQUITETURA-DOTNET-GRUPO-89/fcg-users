using System.Text.Json.Serialization;
using MediatR;
using FcgUsers.Application.Responses.Orders;
using FcgUsers.Domain.ValueObjects;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands.Orders;

public record CreateOrderCommand(
    string Customer,
    decimal TotalAmount,
    string Street,
    string City,
    string State,
    string Cep)
: IRequest<Result<CreateOrderResponse>>, IValidatableRequest
{
    [JsonIgnore]
    public Address DeliveryAddress => new(Street, City, State, Cep);
}
