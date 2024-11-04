namespace Entities.Exceptions;

public sealed class IdParametersBadRequestException() : BadRequestException("Parameters ids is null, please verify.");
