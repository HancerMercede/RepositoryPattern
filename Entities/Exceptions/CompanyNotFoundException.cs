namespace Entities.Exceptions;

public class CompanyNotFoundException(Guid companyId) : NotFoundException($"Company with Id: {companyId} does not exist in the database.");