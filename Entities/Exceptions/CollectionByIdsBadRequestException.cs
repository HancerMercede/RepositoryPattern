namespace Entities.Exceptions;

public class CollectionByIdsBadRequestException()
    : BadRequestException("Collection count mismatch compering to ids, please verify");
