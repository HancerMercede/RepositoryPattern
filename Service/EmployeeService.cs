using Dtos.DtoModels;
using Entities.Exceptions;
using Mapster;

namespace Service;

internal sealed class EmployeeService(IRepositoryManager repositoryManager) : IEmployeeService
{
    #region conventional implementation without Either
    public async Task<Employee> CreateEmployee(string companyId, Employee employee)
    {
        if (employee is null)
            throw new EmployeeBadRequestException();
        
        return  await repositoryManager.Employee.CreateEmployeeForCompany(companyId, employee);
    }
    
    public async Task DeleteEmployee(string companyId, string id, bool trackChanges)
    {
        if (id is null)
            throw new EmployeeNotFoundException(Guid.Parse(id!));

        var company = await repositoryManager.Company.GetByCondition(companyId, trackChanges);
        
        if (company is null)
            throw new CompanyNotFoundException(Guid.Parse(companyId));

        var employee = await repositoryManager.Employee.GetByCondition(companyId, id, trackChanges);

        if (employee is null)
            throw new EmployeeNotFoundException(Guid.Parse(id));
        
        await repositoryManager.Employee.DeleteEmployee(companyId, id, trackChanges);
    }

    public async Task<(EmployeeUpdateDto employeeToPath, Employee employee)> GetEmployeeForPatch(string companyId, string id, bool compTrackChanges, bool empTrackChanges)
    {
         var company = await repositoryManager.Company.GetByCondition(companyId, compTrackChanges);
         
         var existCompany = company ?? throw new CompanyNotFoundException(Guid.Parse(companyId));
         
         var employee = await repositoryManager.Employee.GetByCondition(companyId, id, empTrackChanges);
         
         var existEmployee = employee ?? throw new EmployeeNotFoundException(Guid.Parse(id));

         var employeeToPatch = employee.Adapt<EmployeeUpdateDto>();
         
         return (employeeToPatch, existEmployee);
    }

    public async Task SaveChangesForPatch(EmployeeUpdateDto employee, Employee employeeEntity)
    {
        employee.Adapt(employeeEntity);
        await repositoryManager.Save();
    }
    
    public async Task<(IEnumerable<Employee> Employees, MetaData metaData)> GetAll(string companyId,PaginationParameters paginationParameters, bool trackChanges)
    {
        var employees = await repositoryManager.Employee.GetAll(companyId, paginationParameters, trackChanges);
        var metadata = employees.MetaData;
       
        return (Employees: employees, metaData: metadata); 
    }

    public async Task<Employee> GetByCondition(string companyId, string id, bool trackChanges)
    {
        if (id is null)
            throw new IdParametersBadRequestException();

        var employeeDb = await repositoryManager.Employee.GetByCondition(companyId, id,trackChanges);
        
        if (employeeDb is null)
            throw new EmployeeNotFoundException(Guid.Parse(id));
        
        return employeeDb;
    }
    #endregion
    public EitherAsync<string, (IEnumerable<Employee> employees, MetaData metaData)> GetAllEmployees(string companyId, PaginationParameters pagination, bool trackChanges)
    {
        return EitherAsync<string, string>.FromRight(companyId)
            .Ensure(c => !string.IsNullOrWhiteSpace(c), "The company id can not be null or empty")
            .FlatMap(_ => EitherAsync<string, IEnumerable<Employee>>.Try(async () =>
            {
                var employees = await repositoryManager.Employee.GetAll(companyId, pagination, trackChanges);
                var metadata = employees.MetaData;
                return (Employees: (IEnumerable<Employee>) employees, MetaData: metadata);
            }, exception => exception.Message).Run())
            .Ensure(c=>c.Employees.Any(),"There are no employees for this company.");
    }

    public EitherAsync<string, Employee> GetByConditionWithEither(string companyId, string id, bool trackChanges)
    {
        return EitherAsync<string, string>.FromRight(companyId)
            .Ensure(c => !string.IsNullOrWhiteSpace(c), "The company id can not be null or empty")
            .Ensure(e => !string.IsNullOrWhiteSpace(e), "The employee id can not be null or empty")
            .FlatMap(idParam => EitherAsync<string, Employee>.Try(async () =>
            {
               var employee = await repositoryManager.Employee.GetByCondition(idParam, id, trackChanges);
               return employee;
            }, exception => exception.Message).Run())
            .Ensure(employee=> employee is not null, new EmployeeNotFoundException(Guid.Parse(id)).Message);
    }

    public EitherAsync<string, Employee> CreateEmployeeWithEither(string companyId, Employee employee)
    {
         return EitherAsync<string, string>.FromRight(companyId)
             .Ensure(c=>!string.IsNullOrWhiteSpace(c), "The company id can not be null or empty.")
             .Map(_=>employee)
                 .Ensure(e=>!string.IsNullOrWhiteSpace(e.Name), "The employee name can not be null or empty.")
                 .Ensure(e=>e.Age is >= 18 and <= 80, "The employee age can not be less than 18 or more than 80.")
             .Ensure(e=>!string.IsNullOrWhiteSpace(e.Position), "The employee position can not be null or empty.")
             .FlatMap(_=>EitherAsync<string, Employee>.Try(async () =>
             {
                 var employeeForCompany = await repositoryManager.Employee.CreateEmployeeForCompany(companyId, employee);
                 await repositoryManager.Save();
                 return employeeForCompany;
             },exception=>exception.Message).Run());
    }

    public EitherAsync<string, Unit> DeleteEmployeeWithEither(string companyId, string id, bool trackChanges)
    {
        return EitherAsync<string, string>.FromRight(companyId)
            .Ensure(c => !string.IsNullOrWhiteSpace(c), "The company id can not be null or empty.")
            .Ensure(x => !string.IsNullOrWhiteSpace(x), "The user id can not be null or empty.")
            .FlatMap(_ => EitherAsync<string, Unit>.Try(async () =>
            {
               await repositoryManager.Employee.DeleteEmployee(companyId, id, trackChanges);
               await repositoryManager.Save();
               return new Unit();
            }, exception => exception.Message).Run());
    }
}