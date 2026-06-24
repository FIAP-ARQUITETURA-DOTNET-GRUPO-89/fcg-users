namespace FcgUsers.SharedKernel.Exceptions;

public sealed class NotFoundException(string message) : BusinessException(message);
