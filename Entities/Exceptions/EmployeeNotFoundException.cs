namespace Entities.Exceptions;

public class EmployeeNotFoundException(Guid employeeId) : NotFoundException($"The employee with id {employeeId} does not exist in the database.");
