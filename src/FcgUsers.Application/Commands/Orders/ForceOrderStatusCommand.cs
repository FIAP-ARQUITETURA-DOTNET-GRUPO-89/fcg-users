using System.Text.Json.Serialization;
using MediatR;
using FcgUsers.Application.Responses.Orders;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands.Orders;

public record ForceOrderStatusCommand
    : IRequest<Result<ForceOrderStatusResponse>>, IValidatableRequest
{
    [JsonIgnore]
    public Guid Id { get; set; }

    [JsonIgnore]
    public string? UserId { get; set; }

    [JsonIgnore]
    public bool IsAdmin { get; set; }

    public string NewStatus { get; set; } = string.Empty;
}
