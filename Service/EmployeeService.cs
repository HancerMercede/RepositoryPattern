using Dtos.DtoModels;
using Entities.Exceptions;
using Mapster;

namespace Service;

public class EmployeeService(IRepositoryManager repositoryManager) : IEmployeeService
{
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
    
}