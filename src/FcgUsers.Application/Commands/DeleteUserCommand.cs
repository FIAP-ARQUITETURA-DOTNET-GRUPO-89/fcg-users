using MediatR;
using FcgUsers.Application.Responses.Users;
using FcgUsers.SharedKernel.Validators;
using OperationResult;

namespace FcgUsers.Application.Commands;

public record DeleteUserCommand(Guid Id) : IRequest<Result<DeleteUserResponse>>, IValidatableRequest;
