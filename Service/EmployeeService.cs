using Entities.Exceptions;

namespace Service;

public class EmployeeService:IEmployeeService
{
    private readonly IRepositoryManager _repositoryManager;

    public EmployeeService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public async Task<Employee> CreateEmployee(string companyId, Employee employee)
    {
        if (employee is null)
            throw new EmployeeBadRequestException();
        
        return  await _repositoryManager.Employee.CreateEmployeeForCompany(companyId, employee);
    }

   

    public async Task DeleteEmployee(string companyId, string id, bool trackChanges)
    {
        if (id is null)
            throw new EmployeeNotFoundException(Guid.Parse(id));
        
        await _repositoryManager.Employee.DeleteEmployee(companyId, id, trackChanges);
    }

    public async Task<(IEnumerable<Employee> Employees, MetaData metaData)> GetAll(string companyId,PaginationParameters paginationParameters, bool trackChanges)
    {
        var employees = await _repositoryManager.Employee.GetAll(companyId, paginationParameters, trackChanges);
        var metadata = employees.MetaData;
       
        return (Employees: employees, metaData: metadata); 
    }

    public async Task<Employee> GetByCondition(string companyId, string id, bool trackChanges)
    {
        if (id is null)
            throw new IdParametersBadRequestException();

        var employeeDb = await _repositoryManager.Employee.GetByCondition(companyId, id,trackChanges);
        
        if (employeeDb is null)
            throw new EmployeeNotFoundException(Guid.Parse(id));
        
        return employeeDb;
    }
    
    public async Task SaveChanges()
    {
        await _repositoryManager.Save();
    }
}